using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Bus fmodBus; // Asigna el bus FMOD deseado en el Inspector

    public void SetVolume(float volume)
    {
        fmodBus.setVolume(volume);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
