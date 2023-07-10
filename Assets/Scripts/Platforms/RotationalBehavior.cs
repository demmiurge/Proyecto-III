using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalBehavior : MonoBehaviour
{
    [SerializeField] private Transform pointToRotate;
    [SerializeField] private float speedRotation;
    [SerializeField] private Vector3 axisOfRotation = Vector3.right; // Eje de rotación predeterminado: eje Y


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pointToRotate.Rotate(axisOfRotation, speedRotation * Time.deltaTime);
    }
}
