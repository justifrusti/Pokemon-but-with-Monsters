using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                DropItem();

                hand.gameObject.SetActive(false);
            }

            if (!controllData.lockMouseInEditor)
            {
                controllData.lockMouseInEditor = true;
            }
        }

        if(moveMode && hand.currentItem != null && Input.GetButtonDown("RMB"))
        {
            DropItem();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            moveMode = !moveMode;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) && manager.pc.invSlots[0] != null)
        {
            EquipItem(0);
        }else if(Input.GetKeyDown(KeyCode.Alpha2) && manager.pc.invSlots[1] != null)
        {
            EquipItem(1);
        }else if(Input.GetKeyDown(KeyCode.Alpha3) && manager.pc.invSlots[2] != null)
        {
            EquipItem(2);
        }
    }

    public void MoveItem(InventorySlot inventorySlot)
    {
        if (manager.pc.equippedItem != null)
        {
            Destroy(manager.pc.equippedItem);
            manager.pc.currentItem = null;
        }

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
        if (hand.currentItem != null)
        {
            currentInvItem = hand.currentItem.itemObj;

            Instantiate(currentInvItem, player.transform.position + (player.transform.forward * 2), transform.rotation);

            AmuletCheck(currentInvItem);

            hand.RemoveItem();
            hand.GetComponent<Image>().sprite = defaultImg;
        }
    }

    public void EquipItem(int slotToEquip)
    {
        manager.pc.invIndex = slotToEquip;

        if (manager.pc.equippedItem != null)
        {
            Destroy(manager.pc.equippedItem);
            manager.pc.currentItem = null;
        }

        manager.pc.equippedItem = Instantiate(manager.pc.invSlots[slotToEquip].currentItem.itemObj, manager.pc.holdingHand);
        manager.pc.equippedItem.name = manager.pc.invSlots[slotToEquip].currentItem.itemName;
        manager.pc.equippedItem.GetComponent<ArtifactBehaviour>().isEquiped = true;

        EquipSettings();

        hand.currentItem = null;
    }

    public void EquipSettings()
    {
        Destroy(manager.pc.equippedItem.GetComponent<Rigidbody>());
        manager.pc.equippedItem.GetComponent<Collider>().isTrigger = true;

        if(manager.pc.equippedItem.transform.GetChild(0) != null && manager.pc.equippedItem.transform.GetChild(0).GetComponent<Collider>() != null)
        {
            manager.pc.equippedItem.transform.GetChild(0).GetComponent<Collider>().isTrigger = true;
        }

        manager.pc.currentItem = manager.pc.equippedItem.GetComponent<ItemRef>().item;
    }

    public void AmuletCheck(GameObject go)
    {
        if (go.gameObject.GetComponent<ArtifactBehaviour>().amuletType != ArtifactBehaviour.AmuletType.None)
        {
            switch (go.gameObject.GetComponent<ArtifactBehaviour>().amuletType)
            {
                case ArtifactBehaviour.AmuletType.Health:
                    manager.pc.hasHealthAmulet = false;
                    break;

                case ArtifactBehaviour.AmuletType.Speed:
                    manager.pc.hasSpeedAmulet = false;
                    break;

                case ArtifactBehaviour.AmuletType.Stealth:
                    manager.pc.hasStealthAmulet = false;
                    break;
            }
        }

        manager.pc.UpdateAmuletStats(go.gameObject.GetComponent<ArtifactBehaviour>());
    }
}
