using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdentifier : MonoBehaviour
{
    bool isAdded;

    private EnemyResetter enemyResetter;

    // Start is called before the first frame update
    void Start()
    {
        CheckManagersInstance();
        AddEnemyOnList();
    }

    // Update is called once per frame
    void Update()
    {
        CheckManagersInstance();
    }

    void AddEnemyOnList()
    {
        if (isAdded) return;

        if (GetComponent<EnemigoAereoV2>())
        {
            EnemigoAereoV2 enemy = GetComponent<EnemigoAereoV2>();
            enemyResetter.AddEnemy(enemy);
        }
        if (GetComponent<EnemigoTerrestreV2>())
        {
            EnemigoTerrestreV2 enemy = GetComponent<EnemigoTerrestreV2>();
            enemyResetter.AddEnemy(enemy);
        }
    }

    void CheckManagersInstance()
    {
        if (enemyResetter == null && GameManager.instance.GetEnemyResetter()) enemyResetter = GameManager.instance.GetEnemyResetter();
    }
}
