using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreCollector : MonoBehaviour
{
    [SerializeField] private string scoreTag;
    [SerializeField] private string finisherTag;

    private TotalScoreUI totalScoreUI;
    private GameFinisher gameFinisher;

    void Update()
    {
        if (totalScoreUI == null && GameManager.instance.GetTotalScoreUI()) totalScoreUI = GameManager.instance.GetTotalScoreUI();
        if (gameFinisher == null && GameManager.instance.GetGameFinisher()) gameFinisher = GameManager.instance.GetGameFinisher();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == scoreTag)
            CollectScore(other.gameObject);

        if (other.tag == finisherTag)
            gameFinisher.GameFinish();
    }

    void CollectScore(GameObject scoreGameObject)
    {
        totalScoreUI.AddScore();
        scoreGameObject.SetActive(false);
    }
}
