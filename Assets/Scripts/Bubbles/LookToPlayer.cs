using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToPlayer : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSetting;

    void Update()
    {
        Vector3 direccion = Camera.main.transform.position - transform.position;
        Quaternion rotacion = Quaternion.LookRotation(direccion);
        rotacion *= Quaternion.Euler(rotationSetting);
        transform.rotation = rotacion;
    }
}
