using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour
{
    public Image[] slotIcons;      // 아이콘 이미지
    public Image[] slotBackground; // 배경 노란색 하이라이트
    public Text[] slotCounts;      // [띠또 추가] 아이템 개수 텍스트!!

    public void UpdateQuickSlotUI(ItemData[] slots, int[] counts)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slots[i] != null)
            {
                slotIcons[i].sprite = slots[i].itemIcon;
                slotIcons[i].gameObject.SetActive(true);
                slotIcons[i].color = Color.white;

                if (slotCounts != null && i < slotCounts.Length && slotCounts[i] != null)
                {
                    if (slots[i].type == ItemData.ItemType.Consumable || slots[i].itemName == "Boomb")
                    {
                        slotCounts[i].text = counts[i].ToString(); // 숫자 업데이트
                        slotCounts[i].gameObject.SetActive(true);  // 숫자 켜기
                    }
                    else
                    {
                        slotCounts[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].gameObject.SetActive(false);

                if (slotCounts != null && i < slotCounts.Length && slotCounts[i] != null)
                {
                    slotCounts[i].gameObject.SetActive(false);
                }
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