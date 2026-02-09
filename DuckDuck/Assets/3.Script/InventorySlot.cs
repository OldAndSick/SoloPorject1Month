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
        if (item.itemIcon != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.color = Color.white; 
        }
        else
        {
            itemIcon.color = new Color(0, 0, 0, 0); 
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
