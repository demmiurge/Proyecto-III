using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdentifier : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.SetPlayer(gameObject);
    }
}
