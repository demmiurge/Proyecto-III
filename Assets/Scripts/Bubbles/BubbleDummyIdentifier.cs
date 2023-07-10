using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDummyIdentifier : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.SetBubbleDummy(gameObject);
    }
}
