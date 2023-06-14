using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitSpawner : MonoBehaviour
{
    private void Start()
    {
        if(Random.value > .2f)
        {
            Destroy(gameObject);
        }
    }
}
