using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public static MasterManager Instance { get; private set; }

    //public AudioManager AudioManager { get; private set; }
    //public UIManager UIManager { get; private set; }

    [SerializeField] private BubbleManagerV1 bubbleManager;

    public BubbleManagerV1 GetBubbleManager() => bubbleManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        //AudioManager = GetComponentInChildren<AudioManager>();
        //UIManager = GetComponentInChildren<UIManager>();
    }
}
