using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterBehaviour : MonoBehaviour
{
    public string loadLvlName;

    public float range = 5f;

    public string orbName;

    public GameObject orb;

    private Transform player;
    public PlayerController controller;

    public static TeleporterBehaviour instance;

    public bool inArea;
    private bool placedOrb = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = player.GetComponent<PlayerController>();

        instance = this;

        orb.gameObject.SetActive(false);

        player.GetComponent<PlayerController>().teleporterPos = transform.position;
    }

    private void Update()
    {
        float dst = Vector3.Distance(transform.position, player.position);

        if (dst < range)
        {
            if(placedOrb)
            {
                inArea = true;
            }

            if (controller.currentItem != null)
            {
                if (controller.currentItem.name == orbName)
                {
                    if (Input.GetButtonDown("LMB"))
                    {
                        orb.gameObject.SetActive(true);
                        placedOrb = true;
                        controller.invSlots[controller.invIndex].RemoveItem();
                        Destroy(controller.equippedItem);
                    }
                }
            }
            else 
            {
                Debug.Log("YOU... SHALL... NOT... PASSSSSSSSSS!!!!!!!");
            }
        }else
        {
            inArea = false;
        }
    }
}
