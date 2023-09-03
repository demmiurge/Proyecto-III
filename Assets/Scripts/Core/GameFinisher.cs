using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinisher : MonoBehaviour
{
    private HUDManager hudManager;

    private bool isFinished;
    private float timer;
    private int sequenceStep;
    private bool actionPerformed;

    // Start is called before the first frame update
    void Start()
    {
        isFinished = false;
        timer = 0.0f;
        sequenceStep = 0;
        actionPerformed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hudManager == null && GameManager.instance.GetHUDManager()) hudManager = GameManager.instance.GetHUDManager();

        if (!isFinished) return;
        switch (sequenceStep)
        {
            case 0:
                timer += Time.unscaledDeltaTime;

                if (actionPerformed == false)
                {
                    actionPerformed = true;
                    hudManager.GoToCreditsScreen();
                }

                // END
                isFinished = false;

                break;
        }
    }

    public void ResetEndStatus()
    {
        isFinished = false;
        timer = 0.0f;
        sequenceStep = 0;
        actionPerformed = false;
    }

    public void GameFinish()
    {
        isFinished = true;
    }
}
