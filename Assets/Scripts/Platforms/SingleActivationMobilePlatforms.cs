using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SingleActivationMobilePlatforms : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float speed = 1f;
    [SerializeField] private AnimationCurve movementCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
    [SerializeField] private Transform platform;

    private bool isActive = false;
    private float journeyTime;
    private float currentLerpTime;

    [SerializeField] private UnityEvent onOpen;
    [SerializeField] private UnityEvent onOpened;
    private bool onPerformed;

    public void Activate()
    {
        isActive = true;
        onOpen?.Invoke();
    }

    public void Deactivate()
    {

        isActive = false;
    }

    private void Start()
    {
        // Obtengo el tiempo de viaje mediante la distancia entre las dos posiciones y la velocidad que tendrá la plataforma dada por parámetro
        journeyTime = Vector3.Distance(startPoint.position, endPoint.position) / speed;
    }

    private void Update()
    {
        if (isActive)
        {
            // Si está activo lo movemos al punto final o al punto de muestreo
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime >= journeyTime)
            {
                currentLerpTime = journeyTime;
                onOpened?.Invoke();
            } 
        }
        else
        {
            // Si se desactiva movemos hacia el punto inicial o punto por defecto, siendo este el de partida
            currentLerpTime -= Time.deltaTime;
            if (currentLerpTime <= 0f)
                currentLerpTime = 0f;
        }

        // Lo de siempre, miramos el tiempo del viaje, el tiempo actual de la interpolación entre los dos puntos
        float interpolationValue = currentLerpTime / journeyTime;
        interpolationValue = movementCurve.Evaluate(interpolationValue);
        platform.position = Vector3.Lerp(startPoint.position, endPoint.position, interpolationValue);
    }
}
