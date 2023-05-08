using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Item currentItem;

    public void AddItem(Item itemToAdd)
    {
        currentItem = itemToAdd;
        GetComponent<Image>().sprite = itemToAdd.itemSprite;
    }

    public void RemoveItem()
    {
        currentItem = null;
        GetComponent<Image>().sprite = null;
    }
}
