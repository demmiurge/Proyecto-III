using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackByZone : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter studioEventEmitter;
    [SerializeField] private string identifierParameter = "Zone";

    enum MUSICAL_SETTING
    {
        tutorial = 0,
        exploration = 1,
        temple = 2
    }

    [SerializeField] private MUSICAL_SETTING soundtrackType;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            studioEventEmitter.SetParameter(identifierParameter, Array.IndexOf(Enum.GetValues(typeof(MUSICAL_SETTING)), soundtrackType));
    }
}
