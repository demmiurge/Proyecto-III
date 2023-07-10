using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToGround : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float ballHeight = 1.0f;
    [SerializeField] private bool withoutGravity = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StayOnTheGround();
    }

    void StayOnTheGround()
    {
        if (withoutGravity) return;
            
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            float distanceToGround = hit.distance;
            Vector3 targetPosition = transform.position + (Vector3.down * distanceToGround);
            targetPosition.y += ballHeight;
            transform.position = targetPosition;
        }
    }
}
