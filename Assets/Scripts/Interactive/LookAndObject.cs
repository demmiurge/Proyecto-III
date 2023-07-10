using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAndObject : MonoBehaviour
{
    [SerializeField] private bool lookAtThePlayer = true;
    [SerializeField] private Transform target;

    private Transform playerPosition;

    private void Update()
    {
        CheckManagersInstance(); 

        Vector3 targetPosition;
        Vector3 objectPosition;

        
        if (lookAtThePlayer && playerPosition != null)
            targetPosition = playerPosition.position;
        else
            targetPosition = target.position;

        objectPosition = transform.position;

        Vector3 direction = targetPosition - objectPosition;
        direction.y = 0f;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void CheckManagersInstance()
    {
        if (playerPosition == null && GameManager.instance.GetPlayer()) playerPosition = GameManager.instance.GetPlayer().transform;
    }
}
