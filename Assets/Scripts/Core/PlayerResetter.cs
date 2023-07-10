using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerResetter : MonoBehaviour
{
    [Header("Death sequence times")]
    [SerializeField] private float inGamePlayScreenDuration = 5f;
    [SerializeField] private float deathScreenDuration = 5f;
    [SerializeField] private float blackFadeScreenDuration = 5f;

    [Header("More settings")]
    [SerializeField] private float heartsVanishingTimer = 2.5f;

    private HUDManager hudManager;
    private PlayerLife playerLife;
    private PlayerMovement playerMovement;
    private PlayerRespawn playerRespawn;
    private BubbleManagerV1 bubbleManager;
    private LifeSystemUI lifeSystemUI;
    private EnemyResetter enemyResetter;
    private Animator playerAnimator;
    private TotalScoreUI totalScoreUI;

    private bool isDeath;
    private float timer;
    private int sequenceStep;

    private bool actionPerformed;

    // Start is called before the first frame update
    void Start()
    {
        isDeath = false;
        timer = 0.0f;
        sequenceStep = 0;
        actionPerformed = false;
    }

    void Update()
    {
        if (hudManager == null && GameManager.instance.GetHUDManager()) hudManager = GameManager.instance.GetHUDManager();
        if (playerLife == null && GameManager.instance.GetPlayer()) playerLife = GameManager.instance.GetPlayer().GetComponent<PlayerLife>();
        if (playerRespawn == null && GameManager.instance.GetPlayer()) playerRespawn = GameManager.instance.GetPlayer().GetComponent<PlayerRespawn>();
        if (bubbleManager == null && GameManager.instance.GetBubbleManager()) bubbleManager = GameManager.instance.GetBubbleManager();
        if (enemyResetter == null && GameManager.instance.GetEnemyResetter()) enemyResetter = GameManager.instance.GetEnemyResetter();
        if (lifeSystemUI == null && GameManager.instance.GetLifeSystemUI()) lifeSystemUI = GameManager.instance.GetLifeSystemUI();
        if (playerAnimator == null && GameManager.instance.GetPlayer()) playerAnimator = GameManager.instance.GetPlayer().GetComponent<Animator>();
        if (playerMovement == null && GameManager.instance.GetPlayer()) playerMovement = GameManager.instance.GetPlayer().GetComponent<PlayerMovement>();
        if (totalScoreUI == null && GameManager.instance.GetTotalScoreUI()) totalScoreUI = GameManager.instance.GetTotalScoreUI();

        if (!isDeath) return;
        switch (sequenceStep)
        {
            case 0:
                timer += Time.unscaledDeltaTime;

                if (actionPerformed == false && timer >= heartsVanishingTimer)
                {
                    actionPerformed = true;
                    lifeSystemUI.HideAllHearts();
                    totalScoreUI.HideScoreText();
                }

                if (timer >= inGamePlayScreenDuration)
                {
                    timer = 0f;
                    sequenceStep++;
                    actionPerformed = false;
                }
                break;
            case 1:
                timer += Time.unscaledDeltaTime;

                if (actionPerformed == false)
                {
                    actionPerformed = true;
                    hudManager.GoToDeathScreen();
                }

                if (timer >= deathScreenDuration)
                {
                    timer = 0f;
                    sequenceStep++;
                    actionPerformed = false;
                }
                break;
            case 2:
                timer += Time.unscaledDeltaTime;

                if (actionPerformed == false)
                {
                    actionPerformed = true;
                    hudManager.GoToBlackFadeScreen();
                }
                
                if (timer >= deathScreenDuration)
                {
                    timer = 0f;
                    sequenceStep++;
                    actionPerformed = false;

                    playerLife.RestartLives();
                    playerRespawn.Respawn();
                    bubbleManager.HideAllBubbles();
                    enemyResetter.ResetEnemies();
                    playerAnimator.SetBool("Dead", false);
                    playerMovement.SetMove(true);
                }
                break;
            case 3:
                hudManager.GoToInGamePlayScreen();

                isDeath = false;
                timer = 0.0f;
                sequenceStep = 0;
                actionPerformed = false;
                break;
        }
    }

    public void PlayerDeath()
    {
        isDeath = true;
    }
}
