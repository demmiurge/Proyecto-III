using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotarySkewers : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Transform platform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        platform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
