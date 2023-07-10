using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artefacto : MonoBehaviour
{
    // Velocidad de rotación
    public float rotationSpeed = 20f;

    // Lista de objetos a rotar
    private List<Transform> childrenToRotate = new List<Transform>();

    void Start()
    {
        // Recorremos todos los hijos y los agregamos a la lista
        foreach (Transform child in transform)
        {
            childrenToRotate.Add(child);
        }
    }

    void Update()
    {
        // Rota el GameObject que tiene el script
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Recorremos la lista de hijos y los rotamos uno a uno
        for (int i = 0; i < childrenToRotate.Count; i++)
        {
            // Si el índice es par, rotamos en dirección positiva, si no, en dirección negativa
            float direction = i % 2 == 0 ? 1f : -1f;
            childrenToRotate[i].Rotate(0, direction * rotationSpeed * Time.deltaTime, 0);
        }
    }
}
