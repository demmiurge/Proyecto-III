using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusOnGameObject : MonoBehaviour
{
    [SerializeField] private bool isMainButtonSection;

    private ControllerManager controllerManager;

    private void OnDisable()
    {
        //if (isMainButtonSection)
        //    EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnEnable()
    {
        CheckManagersInstance();

        if (isMainButtonSection && controllerManager.GetControllerIsMouse() == false)
            EventSystem.current.SetSelectedGameObject(gameObject);
        else
            EventSystem.current.SetSelectedGameObject(null);
            
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckManagersInstance();

        if (isMainButtonSection && controllerManager.GetControllerIsMouse() == false)
            EventSystem.current.SetSelectedGameObject(gameObject);
        else
            EventSystem.current.SetSelectedGameObject(null);
    }

    // Update is called once per frame
    void Update()
    {
        CheckManagersInstance();
    }

    void CheckManagersInstance()
    {
        if (controllerManager == null && GameManager.instance.GetControllerManager()) controllerManager = GameManager.instance.GetControllerManager();
    }
}
