using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitterSettings : MonoBehaviour
{
    private StudioEventEmitter studioEventEmitter;

    [SerializeField] private string identifierParameter = "SingleVolume";
    [SerializeField] [Range(0.0f, 1.0f)] private float desiredValue = 1;
    
    private float latestValue;

    void Update()
    {
        if (latestValue != desiredValue) // Ignore precision
        {
            desiredValue = Mathf.Round(desiredValue * 100f) / 100f;
            studioEventEmitter.SetParameter(identifierParameter, desiredValue);
            latestValue = desiredValue;
        }
    }

    void Awake()
    {
        studioEventEmitter = GetComponent<StudioEventEmitter>();
        studioEventEmitter.SetParameter(identifierParameter, desiredValue);
    }

    void Start()
    {
        studioEventEmitter = GetComponent<StudioEventEmitter>();
        studioEventEmitter.SetParameter(identifierParameter, desiredValue);
    }

    public void SetSoundPower()
    {
        studioEventEmitter.SetParameter(identifierParameter, desiredValue);
    }
}
