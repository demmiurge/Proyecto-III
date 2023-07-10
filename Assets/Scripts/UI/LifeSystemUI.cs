using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LifeSystemUI : MonoBehaviour, IBasicUI
{
    private int minLife;
    private int maxLife;
    private int currentLife;

    [SerializeField] private GameObject containerOnLife;
    [SerializeField] private GameObject containerOffLife;

    [SerializeField] private GameObject fullHeartPrefab;
    [SerializeField] private string fullHeartName = "fullHeart";
    [SerializeField] private GameObject emptyHeartPrefab;
    [SerializeField] private string emptyHeartName = "emptyHeart";

    // private List<GameObject> listOfHearts;

    private List<GameObject> livingHeartsList;
    private List<GameObject> lifelessHeartsList;

    void Update()
    {
        CheckManagersInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckManagersInstance();

        livingHeartsList = new List<GameObject>();
        lifelessHeartsList = new List<GameObject>();
        InitializeData();
        CreateHearts();

        foreach (GameObject heart in livingHeartsList)
            heart.SetActive(false);

        foreach (GameObject heart in lifelessHeartsList)
            heart.SetActive(false);

        UpdateUI();
    }

    void InitializeData(int minLife = 0, int maxLife = 3, int currentLife = 3) 
    {
        this.minLife = minLife;
        this.maxLife = maxLife;
        this.currentLife = currentLife;
    }

    void CreateHearts()
    {
        for (int i = 0; i < maxLife; i++)
            livingHeartsList.Add(Instantiate(fullHeartPrefab, containerOnLife.transform));

        for (int i = 0; i < maxLife; i++)
            lifelessHeartsList.Add(Instantiate(emptyHeartPrefab, containerOffLife.transform));
    }

    public void SetDamage(int damage = 1)
    {
        if (currentLife - damage <= minLife)
            currentLife = 0;
        else
            currentLife -= damage;

        UpdateUI();
    }

    public void RemoveAllHearts()
    {
        currentLife = 0;
        UpdateUI();
    }

    public void AddHearts(int heartNumberToAdd = 1)
    {
        if (currentLife + heartNumberToAdd > maxLife)
            currentLife = maxLife;
        else
            currentLife += heartNumberToAdd;

        UpdateUI();
    }

    public void RestarHearts()
    {
        currentLife = maxLife;
        UpdateUI();
    }

    public void UpdateUI()
    {
        DrawHeartsLive(fullHeartName, currentLife);
        DrawHeartsNoLive(emptyHeartName, maxLife - currentLife);
    }

    void DrawHeartsLive(string typeHeart, int numHeartsToDraw)
    {
        int numberOfHeartsDrawn = 0;
        for (int i = 0; i < livingHeartsList.Count; i++)
        {
            if (numberOfHeartsDrawn < numHeartsToDraw && livingHeartsList[i].gameObject.name.Contains(typeHeart))
            {
                livingHeartsList[i].SetActive(true);
                numberOfHeartsDrawn++;
            }
            else
            {
                livingHeartsList[i].GetComponent<PlayAnimation>().OnCallDisappearHear();
                numberOfHeartsDrawn++;
            }
        }
    }

    public void HideAllHearts()
    {
        foreach (GameObject heart in livingHeartsList)
            if (heart.GetComponent<PlayAnimation>())
                heart.GetComponent<PlayAnimation>().OnCallDisappearHear();

        foreach (GameObject heart in lifelessHeartsList)
            if (heart.GetComponent<PlayAnimation>())
                heart.GetComponent<PlayAnimation>().OnCallDisappearHear();
    }

    void DrawHeartsNoLive(string typeHeart, int numHeartsToDraw)
    {
        int numberOfHeartsDrawn = 0;
        for (int i = 0; i < lifelessHeartsList.Count; i++)
        {
            if (numberOfHeartsDrawn < numHeartsToDraw && lifelessHeartsList[i].gameObject.name.Contains(typeHeart))
            {
                lifelessHeartsList[i].SetActive(true);
                numberOfHeartsDrawn++;
            }
            else
            {
                lifelessHeartsList[i].GetComponent<PlayAnimation>().OnCallDisappearHear();
                numberOfHeartsDrawn++;
            }
        }
    }

    void CheckManagersInstance()
    {
        if (GameManager.instance.GetLifeSystemUI() == null) GameManager.instance.SetLifeSystemUI(this);
    }
}
