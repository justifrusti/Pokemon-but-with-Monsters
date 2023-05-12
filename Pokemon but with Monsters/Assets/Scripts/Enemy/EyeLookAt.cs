using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeLookAt : MonoBehaviour
{
    public float range = 5f;
    float dst;
    Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        dst = Vector3.Distance(transform.position, player.position);

        if(dst < range)
        {
            transform.LookAt(player.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
