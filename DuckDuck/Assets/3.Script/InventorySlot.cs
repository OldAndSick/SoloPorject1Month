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
            itemIcon.sprite = item.itemIcon;
        }
    }
    
    public void OnClickSlot()
    {
        if (item == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.AddQuickSlot(item);
        pc.EquipItem(item);
    }
}
