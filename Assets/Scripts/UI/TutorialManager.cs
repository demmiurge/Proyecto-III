using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class TutorialManager : MonoBehaviour
{
    public GameObject canvas;

    private PauseController pauseController;
    private ControllerManager controllerManager;

    private PlayerInputMap input;
    private PlayerInputManage playerInputManage;

    private BoxCollider collider;

    [SerializeField] private Slider slider;
    [SerializeField] private Slider sliderKB;

    private bool ap;

    // Diff

    [SerializeField] private GameObject video01;
    [SerializeField] private GameObject video02;
    [SerializeField] private GameObject video03;

    [SerializeField] private GameObject text01;
    [SerializeField] private GameObject text02;
    [SerializeField] private GameObject text03;

    [SerializeField] private GameObject text01kb;
    [SerializeField] private GameObject text02kb;
    [SerializeField] private GameObject text03kb;

    [SerializeField] private GameObject holder;
    [SerializeField] private GameObject holderKB;

    private int index = 0;

    private void Awake()
    {
        input = new PlayerInputMap();
        input.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (pauseController == null && GameManager.instance.GetPauseController()) pauseController = GameManager.instance.GetPauseController();
        if (controllerManager == null && GameManager.instance.GetControllerManager()) controllerManager = GameManager.instance.GetControllerManager();
        if(playerInputManage == null && GameManager.instance.GetPlayer()) playerInputManage = GameManager.instance.GetPlayer().GetComponent<PlayerInputManage>();

        if (ap)
        {
            slider.value += Time.unscaledDeltaTime;
            sliderKB.value += Time.unscaledDeltaTime;
        }
        else
        {
            slider.value = 0;
            sliderKB.value = 0;
        }
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

            // Activamos el puntero del ratón para la selección de los botones
            controllerManager.ActiveCursor();

            ChangeContent();

            input.UI.Hold.performed += Skip;
            input.UI.Hold.started += Ap;
            input.UI.Hold.canceled += Ap;
        }
    }

    public void NextVideo()
    {
        index++;
        ChangeContent();
    }

    public void LastVideo()
    {
        index--;
        ChangeContent();
    }

    private void ChangeContent()
    {
        switch (index)
        {
            case -1:
                index = 2;
                ChangeContent();
                break;

            case 0:
                HideContent();
                video01.SetActive(true);
                if (playerInputManage.GetGamepadActive() == false)
                {
                    text01kb.SetActive(true);
                }
                else
                {
                    text01.SetActive(true);
                }
                break;

            case 1:
                HideContent();
                video02.SetActive(true);
                if (playerInputManage.GetGamepadActive() == false)
                {
                    text02kb.SetActive(true);
                }
                else
                {
                    text02.SetActive(true);
                }
                break;

            case 2:
                HideContent();
                video03.SetActive(true);
                if (playerInputManage.GetGamepadActive() == false)
                {
                    text03kb.SetActive(true);
                    holderKB.SetActive(true);
                }
                else
                {
                    text03.SetActive(true);
                    holder.SetActive(true);
                }
                break;

            case 3:
                index = 2;
                ChangeContent();
                break;
        }
    }

    private void HideContent()
    {
        video01.SetActive(false);
        video02.SetActive(false);
        video03.SetActive(false);

        text01.SetActive(false);
        text02.SetActive(false);
        text03.SetActive(false);

        text01kb.SetActive(false);
        text02kb.SetActive(false);
        text03kb.SetActive(false);

        holder.SetActive(false);
        holderKB.SetActive(false);
    }

    private void OnDisable()
    {
        input.UI.Hold.performed -= Skip;
        input.Disable();

        input.UI.Hold.started -= Ap;
        input.UI.Hold.canceled -= Ap;
    }

    private void Ap(InputAction.CallbackContext ctx)
    {
        ap = !ap;
    }

    private void Skip(InputAction.CallbackContext ctx)
    {
        //Activate the canvas
        canvas.SetActive(false);

        // Escalamos el tiempo de todos los objetos a 0
        pauseController.ResumeTime();

        // Desactivamos el puntero
        controllerManager.DeactivateCursor();

        collider.enabled = false;

        enabled = false;
    }
}
