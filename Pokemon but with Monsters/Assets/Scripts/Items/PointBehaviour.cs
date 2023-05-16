using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehaviour : MonoBehaviour
{
    public GameObject[] possibleItems;

    [Tooltip("The lower the number the higher the spawn chance!")][Range(0.00f, 1.00f)]public float spawnChance;

    void Start()
    {
        int index = Random.Range(0, possibleItems.Length);

        if (spawnChance !< .05f)
        {
            if (Random.value > spawnChance)
            {
                Instantiate(possibleItems[index], transform.position, possibleItems[index].transform.rotation, transform);
            }
        }else
        {
            Instantiate(possibleItems[index], transform.position, possibleItems[index].transform.rotation, transform);
        }
    }
}
