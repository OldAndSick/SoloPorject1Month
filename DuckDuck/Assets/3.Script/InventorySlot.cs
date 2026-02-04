using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemIcon;
    public Text itemNameText;
    private ItemData item;

    public void SetItem(ItemData newItem)
    {
        item = newItem;
        if(item != null)
        {
            itemNameText.text = item.itemName;
        }
    }
    public void OnClickSlot()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().EquipItem(item);
    }
}
