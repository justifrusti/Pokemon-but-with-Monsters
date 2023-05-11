using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        itemManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemManager>();
    }

    private void Start()
    {
        if(itemManager.items.Count != 0)
        {
            int index = Random.Range(0, itemManager.items.Count);

            Instantiate(itemManager.items[index].itemObj, itemSpawnPoint.position, Quaternion.identity);

            itemManager.items.RemoveAt(index);
        }else
        {
            Destroy(gameObject);
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
