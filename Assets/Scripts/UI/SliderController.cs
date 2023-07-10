using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    enum SOUND_CLASSIFICATION
    {
        MUSIC,
        SFX,
        MASTER
    }

    [SerializeField] private SOUND_CLASSIFICATION soundClassification;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private VolumeController volumeControl;
    
    private AudioSettings audioSettings;

    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void Update()
    {
        if (audioSettings == null && GameManager.instance.GetAudioSettings())
        {
            audioSettings = GameManager.instance.GetAudioSettings();
            switch (soundClassification)
            {
                case SOUND_CLASSIFICATION.MUSIC:
                    volumeSlider.SetValueWithoutNotify(audioSettings.GetMusicVolumeLevel());
                    break;
                case SOUND_CLASSIFICATION.SFX:
                    volumeSlider.SetValueWithoutNotify(audioSettings.GetSFXVolumeLevel());
                    break;
                case SOUND_CLASSIFICATION.MASTER:
                    volumeSlider.SetValueWithoutNotify(audioSettings.GetMasterVolumeLevel());
                    break;
            }
        }
    }

    private void OnVolumeChanged(float volume)
    {
        switch (soundClassification)
        {
            case SOUND_CLASSIFICATION.MUSIC:
                audioSettings.SetMusicVolumeLevel(volume);
                break;
            case SOUND_CLASSIFICATION.SFX:
                audioSettings.SetSFXVolumeLevel(volume);
                break;
            case SOUND_CLASSIFICATION.MASTER:
                audioSettings.SetMasterVolumeLevel(volume);
                break;
        }
    }
}
