using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehaviour : MonoBehaviour
{
    public enum ChestType
    {
        Area,
        Orb
    }

    public ChestType chestType;

    public Transform player;
    public Transform chestTop;
    public Transform itemSpawnPoint;

    public ItemManager itemManager;

    public float range = 2f;

    private bool playedSound;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        itemManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<ItemManager>();
    }

    private void Start()
    {
        switch(chestType)
        {
            case ChestType.Area:
                if (itemManager.items.Count != 0)
                {
                    if(Random.value < .2f)
                    {
                        int index = Random.Range(0, itemManager.items.Count);

                        Vector3 newSpawnPos = new Vector3(itemSpawnPoint.position.x, itemSpawnPoint.position.y + .15f, itemSpawnPoint.position.z);
                        GameObject g = Instantiate(itemManager.items[index].itemObj, newSpawnPos, Quaternion.identity);

                        itemManager.items.RemoveAt(index);

                        if (g.GetComponent<ItemRef>().item.destroyChestAroundArtifact)
                        {
                            Destroy(gameObject);
                        }
                    }else
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
                break;

            case ChestType.Orb:
                Instantiate(itemManager.orb.itemObj, itemSpawnPoint.position, Quaternion.identity);

                player.GetComponent<PlayerController>().orbPos = itemSpawnPoint.position;
                break;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= range)
        {
            if(!playedSound)
            {
                playedSound = true;

                AudioManager.instance.PlayClip("Open Chest");
            }

            Quaternion targetRotation = Quaternion.Euler(-110f, chestTop.rotation.eulerAngles.y, chestTop.rotation.eulerAngles.z);

            chestTop.rotation = Quaternion.Lerp(chestTop.rotation, targetRotation, .35f * Time.deltaTime);

            if(Mathf.Approximately(chestTop.rotation.eulerAngles.x, 270f))
            {
                range = 0;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + .25f, transform.position.z), range);
    }
}
