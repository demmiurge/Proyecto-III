using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlatform : MonoBehaviour
{
    private enum BUTTON_TYPE
    {
        SET_OF_BUTTONS,
        SINGLE_BUTTON,
        AND_GATE,
    }

    // Tipo de bot�n que soy
    [SerializeField] private BUTTON_TYPE buttonType;

    // Indicar quien me puede activar
    [SerializeField] private List<string> tagsThatActivateMe;

    // �Seguir� activo si el jugador sale de la plataforma?
    [SerializeField] private bool staysActiveOnExit;

    [SerializeField] private List<PressurePlatform> neighborButtons;

    private float timeToReset = 0.125f;
    private float restTime;

    [SerializeField] private UnityEvent eventActivated;
    [SerializeField] private UnityEvent eventDisabled;

    private int numberOfObjects;

    private bool areActivatingMe;

    private void ActivateEvent()
    {
        eventActivated?.Invoke();
    }

    private void DisableEvent()
    {
        eventDisabled?.Invoke();
    } 

    public bool IsActive() => areActivatingMe;

    // Start is called before the first frame update
    void Start()
    {
        numberOfObjects = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (areActivatingMe && staysActiveOnExit == false) 
        {
            if (restTime > 0)
            {
                restTime -= Time.deltaTime;
            }
            else
            {
                numberOfObjects = 0;
                areActivatingMe = false;
                DisableEvent();
            }
        }
    }

    private bool CheckIfTheObjectIsValid(Collider other)
    {
        bool invalidObject = true;

        foreach (string tag in tagsThatActivateMe)
        {
            if (tag == other.tag)
                invalidObject = false;
        }

        return invalidObject;
    }

    // Si me han activado y soy una placa de presi�n que permanece siempre activa
    private bool IWillAlwaysBeActive()
    {
        // Comprobaci�n especial en caso de una puerta Y
        // Esta acci�n se da en la comprobaci�n del OnTriggerStay
        // 
        if (buttonType == BUTTON_TYPE.AND_GATE && areActivatingMe && staysActiveOnExit)
        {
            foreach (PressurePlatform button in neighborButtons)
                if (button.IsActive() == false)
                    return false;

            return true;
        }
        
        return areActivatingMe && staysActiveOnExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si ya estoy activada no hago nada
        if (IWillAlwaysBeActive()) return;

        // Primero compruebo que el "collider" que ha entrado en mi zona de colisi�n pueda activarme
        if (CheckIfTheObjectIsValid(other)) return;

        // Como es v�lido, pues me activo
        areActivatingMe = true;

        // Sumo uno al contador de objetos que tengo encima
        numberOfObjects++;

        switch (buttonType)
        {
            case BUTTON_TYPE.SET_OF_BUTTONS:
                // Realizo las acciones pertinentes comprobando mis botones vecinos
                MeasurementsWithButtonSet();
                break;
            case BUTTON_TYPE.SINGLE_BUTTON:
                // Emito mi acci�n, como bot�n singular
                MeasurementsForOnlyOneButton();
                break;
            case BUTTON_TYPE.AND_GATE:
                MeasuramentsForANDGate();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Si ya estoy activada no hago nada
        if (IWillAlwaysBeActive()) return;

        // Primero compruebo que el "collider" que ha salido de la zona de colisi�n pueda desactivarme
        if (CheckIfTheObjectIsValid(other)) return;

        // Introducimos nuevamente el timer
        restTime = timeToReset;
    }

    private void OnTriggerExit(Collider other)
    {
        // Si ya estoy activada no hago nada
        if (IWillAlwaysBeActive()) return;

        // Primero compruebo que el "collider" que ha salido de la zona de colisi�n pueda desactivarme
        if (CheckIfTheObjectIsValid(other)) return;

        // Resto uno al contador de objetos que tengo encima
        numberOfObjects--;

        // Pregunto si hay un total de cero objetos para informar de la desactivaci�n
        if (numberOfObjects == 0) 
        {
            // Si no tengo nada me desactivo y emito evento para informar
            areActivatingMe = false;
            DisableEvent();
        }
    }

    private void MeasurementsWithButtonSet()
    {
        bool allTheButtonsAreBeingPushed = true;

        // Si todos mis vecinos est�n activos me mantendr� siempre pulsado
        foreach (PressurePlatform button in neighborButtons)
            if (button.IsActive() == false)
                allTheButtonsAreBeingPushed = false;

        // Si est�n todos los botones activados, congelo mi activaci�n
        if (allTheButtonsAreBeingPushed)
        {
            staysActiveOnExit = true;
            // Les digo a todos mis vecinos que se activen permanentemente
            foreach (PressurePlatform button in neighborButtons)
                button.staysActiveOnExit = true;
        }

        // Despu�s de hacer toda la matraca con mis botones vecinos me activo yo, porque una se lo merece
        MeasurementsForOnlyOneButton();
    }

    private void MeasurementsForOnlyOneButton()
    {
        ActivateEvent();
    }

    private void MeasuramentsForANDGate()
    {
        bool allTheButtonsAreBeingPushed = true;

        // Primero compruebo si todos los botones vecinos est�n activados
        foreach (PressurePlatform button in neighborButtons)
            if (button.IsActive() == false)
                allTheButtonsAreBeingPushed = false;
        
        // Si alguno no est� activado, salgo de la acci�n
        if (allTheButtonsAreBeingPushed == false) return;

        // Si todos los botones est�n activados lo activo
        MeasurementsForOnlyOneButton();
    }
}
