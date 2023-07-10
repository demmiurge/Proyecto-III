using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResetter : MonoBehaviour
{
    private List<EnemigoAereoV2> listOfAirEnemies = new List<EnemigoAereoV2>();
    private List<EnemigoTerrestreV2> listOfGroundEnemies = new List<EnemigoTerrestreV2>();

    public void AddEnemy(EnemigoAereoV2 enemy)
    {
        listOfAirEnemies.Add(enemy);
    }

    public void AddEnemy(EnemigoTerrestreV2 enemy)
    {
        listOfGroundEnemies.Add(enemy);
    }

    public void ResetEnemies()
    {
        foreach (EnemigoAereoV2 enemy in listOfAirEnemies)
            enemy.RestartGameEnemy();

        foreach (EnemigoTerrestreV2 enemy in listOfGroundEnemies)
            enemy.RestartGameEnemy();
    }
}
