using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUp : MonoBehaviour
{
    public float range = 3f;
    public float uppyThingy = 1.7f;

    Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float dst = Vector3.Distance(player.position, transform.position);

        if(dst < range)
        {
            transform.position += new Vector3(0, uppyThingy, 0) * Time.deltaTime;
        }
    }
}
