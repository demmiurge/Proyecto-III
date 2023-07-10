using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VideoPlayer : MonoBehaviour
{
    //The canvas you want to enable
    public GameObject canvas;
    private PauseController pauseController;
    private PlayerInputMap input;
    private Collider collider;
    public Slider slider;
    private bool ap;
    private PlayerInputManage playerInput;

    [SerializeField] private GameObject video01;
    [SerializeField] private GameObject video02;
    [SerializeField] private GameObject video03;

    private void Start()
    {
        if (pauseController == null && GameManager.instance.GetPauseController()) pauseController = GameManager.instance.GetPauseController();
        collider = GetComponent<Collider>();
    }

    private void Awake()
    {
        input = new PlayerInputMap();
        
        input.Enable();
        
    }

    private void Update()
    {
        if (ap)
        {
            slider.value += Time.unscaledDeltaTime;
        }
        else
        {
            slider.value = 0;
        }
    }

    private void Skip(InputAction.CallbackContext ctx)
    {
        //Activate the canvas
        canvas.SetActive(false);
        // Escalamos el tiempo de todos los objetos a 0
        pauseController.ResumeTime();

        collider.enabled = false;

        enabled = false;

       
    }

    private void Ap(InputAction.CallbackContext ctx)
    {
        ap = !ap;
    }

    private void OnDisable()
    {
        input.UI.Hold.performed -= Skip;
        input.Disable();

        input.UI.Hold.started -= Ap;
        input.UI.Hold.canceled -= Ap;
    }

    //Check if something has entered the collider this script is on
    private void OnTriggerEnter(Collider other)
    {
        
        //Check if the object has the tag car
        if (other.tag == "Player")
        {
            //Activate the canvas
            canvas.SetActive(true);
            // Escalamos el tiempo de todos los objetos a 0
            pauseController.PauseTime();
            input.UI.Hold.performed += Skip;
            input.UI.Hold.started += Ap;
            input.UI.Hold.canceled += Ap;
            

        }
    }

    
}

