using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalState : MonoBehaviour
{
    private BubbleBehaviourV1 bubbleBehaviour;
    private bool iHaveElectricity { get; set; }

    private List<string> connectionTags;

    public bool GetHaveElectricity() => iHaveElectricity;

    [SerializeField] private ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        bubbleBehaviour = GetComponent<BubbleBehaviourV1>();

        connectionTags = new List<string>();
        connectionTags = bubbleBehaviour.GetStickySurfaces();
    }

    void OnEnable()
    {
        iHaveElectricity = false;
        if (!particleSystem || !transform) return;
        particleSystem.Stop();
        particleSystem.gameObject.transform.parent = transform;
        particleSystem.gameObject.transform.localPosition = Vector3.zero;
    }

    void OnDisable()
    {
        iHaveElectricity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bubble")
        {
            if (other?.gameObject.GetComponent<ElectricalState>())
            {
                ElectricalState electricalState = other?.gameObject.GetComponent<ElectricalState>();

                if (electricalState.iHaveElectricity)
                {
                    iHaveElectricity = true;
                    if (!particleSystem || !transform) return;
                    particleSystem.Play();
                    particleSystem.gameObject.transform.parent = transform;
                    particleSystem.gameObject.transform.localPosition = Vector3.zero;
                }
            }
        }

        if (CheckIsHaveTag(other.tag))
        {
            if (other?.gameObject.GetComponent<ElectricityGenerator>())
            {
                ElectricityGenerator electricityGenerator = other?.gameObject.GetComponent<ElectricityGenerator>();

                if (electricityGenerator.GetIsElectricityGenerator())
                {
                    iHaveElectricity = true;
                    particleSystem.Play();
                }
            }
        }
    }

    private bool CheckIsHaveTag(string tag)
    {
        foreach (string connectionTag in connectionTags)
        {
            if (tag == connectionTag)
            {
                return true;
            }
        }
        return false;
    }
}
