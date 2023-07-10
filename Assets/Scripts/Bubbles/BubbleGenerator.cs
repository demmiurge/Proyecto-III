using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGenerator : MonoBehaviour
{
    [SerializeField] private Transform bubbleSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBubble(GameObject bubble)
    {
        Rigidbody rbBubble = bubble.GetComponent<Rigidbody>();

        rbBubble.isKinematic = true;

        bubble.transform.position = bubbleSpawnPoint.transform.position;

        rbBubble.isKinematic = false;
    }
}
