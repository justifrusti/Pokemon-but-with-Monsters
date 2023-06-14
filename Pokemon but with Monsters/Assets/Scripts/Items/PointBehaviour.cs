using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehaviour : MonoBehaviour
{
    public GameObject[] possibleItems;

    void Start()
    {
        if(Random.value > .3f)
        {
            int index = Random.Range(0, possibleItems.Length);

            Instantiate(possibleItems[index], transform.position, possibleItems[index].transform.rotation, transform);
        }
    }
}
