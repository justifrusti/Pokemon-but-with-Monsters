using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public List<GameObject> spawnableItems;

    public static PointManager instance;

    private void Awake()
    {
        instance = this;
    }
}
