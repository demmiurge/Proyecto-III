using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElectricityGenerator : MonoBehaviour
{
    [SerializeField] private bool isElectricityGenerator;
    public bool GetIsElectricityGenerator() => isElectricityGenerator;

    public UnityEvent iAmActivated;
    public UnityEvent iAmDisabled;

    private bool iGetElectricity;
    private ParticleSystem particleSystem;

    [SerializeField] private UnityEvent onActive;
    [SerializeField] private UnityEvent onDisable;

    // Start is called before the first frame update
    void Start()
    {
        iGetElectricity = false;
        //particleSystem = GetComponent<ParticleSystem>();
        if (isElectricityGenerator)
        {
            //particleSystem.Play();
            onActive?.Invoke();
        }
        else
        {
            onDisable?.Invoke();
            //particleSystem.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            iAmActivated?.Invoke();
        }
    }

    public void ShutDownGenerator()
    {
        if (isElectricityGenerator) return;

        iGetElectricity = false;
        iAmDisabled?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isElectricityGenerator) return;

        if (other.tag == "Bubble")
        {
            if (other?.GetComponent<ElectricalState>())
            {
                ElectricalState electricalState = other.GetComponent<ElectricalState>();

                if (electricalState.GetHaveElectricity())
                {
                    iGetElectricity = true;
                    //particleSystem.Play();
                    onActive?.Invoke();
                    iAmActivated?.Invoke();
                }
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (isElectricityGenerator) return;

        if (other.tag == "Bubble")
        {
            iGetElectricity = false;
            iAmDisabled?.Invoke();
        }
    }
}
