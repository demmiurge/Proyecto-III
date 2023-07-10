using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProjectionHelper : MonoBehaviour
{
    [SerializeField] private bool perform = true;
    [SerializeField] private float raycastDistance = 5f;
    [SerializeField] private float offset = 0.0125f;

    void Start()
    {
        if (perform != true) return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
        {
            Vector3 targetPosition = hit.point + hit.normal * offset;
            transform.position = targetPosition;
            transform.LookAt(hit.point - hit.normal);
        }
    }
}
