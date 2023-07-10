using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalScoreUI : MonoBehaviour, IBasicUI
{
    private int currentScore;

    [SerializeField] private int totalScore = 0;
    [SerializeField] private TextMeshProUGUI scoreTextMeshPro;

    void Awake()
    {
        
    }

    void Start()
    {
        currentScore = 0;
        GameManager.instance.SetTotalScoreUI(this);
        UpdateUI();
    }

    public void AddScore()
    {
        scoreTextMeshPro.GetComponent<PlayAnimation>().ActivateSpecialEvent();
        currentScore++;
        UpdateUI();
    }

    public void LoadCheckpoint(int desiredScore)
    {
        currentScore = desiredScore;
        UpdateUI();
    }

    public void HideScoreText()
    {
        scoreTextMeshPro.GetComponent<PlayAnimation>().OnCallDisappearHear();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateUI();
    }

    public void UpdateUI()
    {
        scoreTextMeshPro.SetText(currentScore.ToString() + "/" + totalScore);
    }
}
