using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float destroyTime;

    private void Start()
    {
        Invoke("StartDestroy", destroyTime);
    }

    public void StartDestroy()
    {
        Destroy(gameObject);
    }
}
