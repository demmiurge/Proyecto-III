using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private PauseController pauseController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseController == null && GameManager.instance.GetPauseController()) pauseController = GameManager.instance.GetPauseController();

        pauseController.PauseTime();
        pauseController.ResumeTime();
    }
}
