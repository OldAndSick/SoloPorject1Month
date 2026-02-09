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
        Debug.Log("UpdateQuickSlotUI 함수 진입 성공!");
        for (int i = 0; i< slotIcons.Length; i++)
        {
            if (slot[i] != null && slot[i].itemIcon != null)
            {
                slotIcons[i].sprite = slot[i].itemIcon;
                slotIcons[i].gameObject.SetActive(true);

                slotIcons[i].color = Color.white;
                Debug.Log($"{i}번 퀵슬롯에 {slot[i].itemName} 그리기 완료!");
            }
            else
            {
                slotIcons[i].gameObject.SetActive(false);
                Debug.LogError($"{slot[i].itemName} 데이터에 아이콘 사진이 없다 이놈아!");
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
