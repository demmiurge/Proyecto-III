using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugDevop : MonoBehaviour
{
    public KeyCode[] combo;
    public int currentIndex = 0;
    public float interval = 1.0f; // Intervalo de tiempo en segundos


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ClearLog", 0, interval);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentIndex < combo.Length)
        {
            if (Input.GetKeyDown(combo[currentIndex]))
                currentIndex++;
        }
        else
            GameManager.instance.ResetScene();
    }

    void ClearLog()
    {
        currentIndex = 0;
    }
}
