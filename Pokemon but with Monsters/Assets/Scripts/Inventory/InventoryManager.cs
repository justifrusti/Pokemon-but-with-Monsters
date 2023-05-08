using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot hand;

    public GameObject currentInvItem;
    public GameObject player;

    public Sprite defaultImg;

    public GameManager manager;

    private bool moveMode;
    private ControllerData controllData;

    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        controllData = player.GetComponent<PlayerController>().playerControlData;
    }

    private void Update()
    {
        if(moveMode)
        {
            if(!hand.gameObject.activeInHierarchy)
            {
                hand.gameObject.SetActive(true);
            }

            if(controllData.lockMouseInEditor)
            {
                controllData.lockMouseInEditor = false;
            }
        }else
        {
            if(hand.gameObject.activeInHierarchy)
            {
                hand.gameObject.SetActive(false);
            }

            if (!controllData.lockMouseInEditor)
            {
                controllData.lockMouseInEditor = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            moveMode = !moveMode;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipItem(0);
        }else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipItem(1);
        }else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipItem(2);
        }
    }

    public void MoveItem(InventorySlot inventorySlot)
    {
        if (inventorySlot.currentItem != null && hand.currentItem == null)
        {
            hand.AddItem(inventorySlot.currentItem);

            inventorySlot.RemoveItem();
            inventorySlot.GetComponent<Image>().sprite = defaultImg;
        }
        else if (inventorySlot.currentItem == null && hand.currentItem != null)
        {
            inventorySlot.AddItem(hand.currentItem);

            hand.RemoveItem();
            hand.GetComponent<Image>().sprite = defaultImg;
        }
        else if (inventorySlot.currentItem != null && hand.currentItem != null)
        {
            InventorySlot temp = new InventorySlot();

            temp.currentItem = inventorySlot.currentItem;
            inventorySlot.AddItem(hand.currentItem);

            hand.AddItem(temp.currentItem);
        }
    }

    public void DropItem()
    {
        if (hand.currentItem != null && moveMode)
        {
            currentInvItem = hand.currentItem.itemObj;

            Instantiate(currentInvItem, player.transform.position + (player.transform.forward * 2), transform.rotation);

            hand.RemoveItem();
            hand.GetComponent<Image>().sprite = defaultImg;
        }
    }

    public void EquipItem(int slotToEquip)
    {
        if (manager.pc.equippedItem != null)
        {
            Destroy(manager.pc.equippedItem);
            manager.pc.currentItem = null;
        }

        manager.pc.equippedItem = Instantiate(manager.pc.invSlots[slotToEquip].currentItem.itemObj, manager.pc.holdingHand);

        EquipSettings();

        hand.currentItem = null;
    }

    public void EquipSettings()
    {
        Destroy(manager.pc.equippedItem.GetComponent<Rigidbody>());
        manager.pc.equippedItem.GetComponent<Collider>().isTrigger = true;

        manager.pc.currentItem = manager.pc.equippedItem.GetComponent<ItemRef>().item;
    }
}
