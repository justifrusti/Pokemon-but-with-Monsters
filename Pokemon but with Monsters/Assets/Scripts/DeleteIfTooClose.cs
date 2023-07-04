using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteIfTooClose : MonoBehaviour
{
    private void Start()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        float dstToPlayer = Vector3.Distance(transform.position, player.position);

        if(dstToPlayer < 30)
        {
            Destroy(gameObject);
        }
    }
}
