using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterBehaviour : MonoBehaviour
{
    public float range = 5f;

    public string orbName;

    public GameObject orb;

    private Transform player;
    public PlayerController controller;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = player.GetComponent<PlayerController>();

        orb.gameObject.SetActive(false);
    }

    private void Update()
    {
        float dst = Vector3.Distance(transform.position, player.position);

        if (dst < range)
        {
            if (controller.currentItem != null)
            {
                if (controller.currentItem.name == orbName)
                {
                    if (Input.GetButtonDown("LMB"))
                    {
                        orb.gameObject.SetActive(true);
                        controller.invSlots[controller.invIndex].RemoveItem();
                    }
                }
            }
            else 
            {
                Debug.Log("YOU... SHALL... NOT... PASSSSSSSSSS!!!!!!!");
            }
        }
    }
}
