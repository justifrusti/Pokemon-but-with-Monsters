using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Item currentItem;

    public Sprite sprite;

    private void Start()
    {
        sprite = GetComponent<Image>().sprite;
    }

    public void AddItem(Item itemToAdd)
    {
        currentItem = itemToAdd;
        GetComponent<Image>().sprite = itemToAdd.itemSprite;
    }

    public void RemoveItem()
    {
        currentItem = null;
        GetComponent<Image>().sprite = sprite;
    }
}
