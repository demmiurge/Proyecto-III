using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElectricityGeneratorV1 : MonoBehaviour
{
    [SerializeField] private List<string> stickySurfaces;

    [SerializeField] private bool iAmTheGenerator;

    [SerializeField] private float broadcastTimer;

    private bool emitElectricity;

    private float timePassed;

    public bool GetIAmTheGenerator() => iAmTheGenerator;

    private List<GameObject> connections;

    // Start is called before the first frame update
    void Start()
    {
        connections = new List<GameObject>();
        timePassed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        ElectricityEmitter();
    }

    private void ElectricityEmitter()
    {
        if (emitElectricity == false) return;
        
        timePassed += Time.deltaTime;

        if (timePassed > broadcastTimer)
        {
            EmitElectricityFromTheObjects();
        }
    }

    private void EmitElectricityFromTheObjects()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (string stickySurface in stickySurfaces)
        {
            if (stickySurface == other.tag)
            {
                // Comprobamos si ya la teníamos en la lista, para evitar la duplicidad
                foreach (GameObject connection in connections)
                {
                    if (connection == other.gameObject)
                    {
                        return;
                    }
                }

                // Tipos de casos
                // Agregamos la burbuja a nuestra lista al recibir un impacto
                connections.Add(other.gameObject);

                // Al terminar con todas las acciones hacemos la comprobación
                CheckConnectionsForEmission();
            }
        }
    }

    private void CheckConnectionsForEmission()
    {
        emitElectricity = connections.Count > 0;
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (string stickySurface in stickySurfaces)
        {
            if (stickySurface == other.tag)
            {
                connections.Remove(other.gameObject);

                // Al terminar con todas las acciones hacemos la comprobación
                CheckConnectionsForEmission();
            }
        }
    }

    bool AreYouAReceiver(GameObject gameObject)
    {
        return gameObject.GetComponent<ElectricityGeneratorV1>().GetIAmTheGenerator();
    }
}
