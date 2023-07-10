using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    private FMOD.Studio.Bus busMusic;
    private FMOD.Studio.Bus busSFX;
    private FMOD.Studio.Bus busMaster;
    private float busMusicVolume = 0.75f;
    private float busSFXVolume = 0.75f;
    private float busMasterVolume = 1f;

    void Awake()
    {
        busMusic = RuntimeManager.GetBus("bus:/Master/Music");
        busSFX = RuntimeManager.GetBus("bus:/Master/SFX");
        busMaster = RuntimeManager.GetBus("bus:/Master");
    }

    // Update is called once per frame
    void Update()
    {
        busMusic.setVolume(busMusicVolume);
        busSFX.setVolume(busSFXVolume);
        busMaster.setVolume(busMasterVolume);
    }

    public float GetMusicVolumeLevel() => busMusicVolume;

    public void SetMusicVolumeLevel(float newMusicVolume)
    {
        busMusicVolume = newMusicVolume;
    }

    public float GetSFXVolumeLevel() => busSFXVolume;

    public void SetSFXVolumeLevel(float newSFXVolume)
    {
        busSFXVolume = newSFXVolume;
    }

    public float GetMasterVolumeLevel() => busMasterVolume;

    public void SetMasterVolumeLevel(float newMasterVolume)
    {
        busMasterVolume = newMasterVolume;
    }

    public void StopAllSounds()
    {
        FMOD.Studio.Bus masterBus = RuntimeManager.GetBus("bus:/");
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
