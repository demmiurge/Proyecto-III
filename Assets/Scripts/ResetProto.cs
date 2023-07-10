using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetProto : MonoBehaviour
{
    public GameObject player;
    public GameObject spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        player.transform.position = spawnPoint.transform.position;
        
        
    }
}
