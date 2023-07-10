using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
    public float speed = 1.0f;
    public float height = 0.5f;

    private float originalY;


    void Start()
    {
        originalY = transform.localPosition.y;
    }

    void Update()
    {
        Vector3 v = transform.localPosition;
        v.y = originalY + height * Mathf.Sin(Time.time * speed);
        transform.localPosition = v;
    }
}