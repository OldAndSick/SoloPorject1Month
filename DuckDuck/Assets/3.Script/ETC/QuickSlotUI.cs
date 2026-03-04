using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour
{
    public Image[] slotIcons;      // 아이콘 이미지
    public Image[] slotBackground; // 배경 노란색 하이라이트
    public Text[] slotCounts;      // [띠또 추가] 아이템 개수 텍스트!! ⭐

    // [중요] 인자값에 int[] counts를 추가해서 개수 데이터를 받습니다.
    public void UpdateQuickSlotUI(ItemData[] slot, int[] counts)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            // [방어코드] 슬롯 데이터가 없으면 그냥 다음으로!
            if (slot == null || i >= slot.Length) continue;

            if (slot[i] != null)
            {
                // 1. 아이콘 표시
                if (slotIcons[i] != null && slot[i].itemIcon != null)
                {
                    slotIcons[i].sprite = slot[i].itemIcon;
                    slotIcons[i].gameObject.SetActive(true);
                    slotIcons[i].color = Color.white;
                }

                // 2. 개수 표시 (에러 방지용 null 체크 강화)
                // slotCounts 배열 자체가 없거나, i번째 칸이 비어있으면 무시합니다.
                if (slotCounts != null && i < slotCounts.Length && slotCounts[i] != null)
                {
                    if (slot[i].type == ItemData.ItemType.Consumable || slot[i].itemName == "Boomb")
                    {
                        slotCounts[i].text = counts[i].ToString();
                        slotCounts[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        slotCounts[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (slotIcons[i] != null) slotIcons[i].gameObject.SetActive(false);
                if (slotCounts != null && i < slotCounts.Length && slotCounts[i] != null)
                    slotCounts[i].gameObject.SetActive(false);
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