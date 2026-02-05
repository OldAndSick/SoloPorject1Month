using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour
{
    public Image[] slotIcons;
    public Image[] slotBackground;
    public void UpdateQuickSlotUI(ItemData[] slot)
    {
        for(int i = 0; i< slotIcons.Length; i++)
        {
            if (slot[i] != null && slot[i].itemIcon != null)
            {
                slotIcons[i].sprite = slot[i].itemIcon;
                slotIcons[i].gameObject.SetActive(true);
            }
            else
            {
                slotIcons[i].gameObject.SetActive(false);
            }
        }
    }
    public void HighlightSlot(int index)
    {
        for (int i = 0; i < slotBackground.Length; i++)
        {
            slotBackground[i].color = (i == index) ? Color.yellow : Color.white;
        }
    }
}
