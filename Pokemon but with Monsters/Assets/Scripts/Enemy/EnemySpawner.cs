using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyBehaviour> spawnableEnemies;

    public static EnemySpawner instance;

    public List<Transform> spawnPoints;

    private void Start()
    {
        instance = this;
    }

    public void SpawnEnemies()
    {
        RegisterPoints();

        int enemiesToSpawn = 0;

        if (spawnableEnemies.Count < 3 && spawnableEnemies.Count != 0)
        {
            enemiesToSpawn = Random.Range(2, spawnableEnemies.Count);
        }
        else
        {
            enemiesToSpawn = Random.Range(2, 4);
        }

        if(spawnableEnemies.Count == 1)
        {
            int spawnPointIndex = Random.Range(0, spawnPoints.Count);

            Instantiate(spawnableEnemies[0].gameObject, spawnPoints[spawnPointIndex].position, Quaternion.identity);

            spawnableEnemies.RemoveAt(0);
            spawnPoints.RemoveAt(spawnPointIndex);
        }else if(spawnableEnemies.Count > 1)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                int enemyIndex = Random.Range(0, spawnableEnemies.Count);
                int spawnPointIndex = Random.Range(0, spawnPoints.Count);

                Instantiate(spawnableEnemies[enemyIndex].gameObject, spawnPoints[spawnPointIndex].position, Quaternion.identity);

                spawnableEnemies.RemoveAt(enemyIndex);
                spawnPoints.RemoveAt(spawnPointIndex);
            }
        }
    }

    void RegisterPoints()
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("RoamingPoint"))
        {
            spawnPoints.Add(item.transform);
        }
    }
}
