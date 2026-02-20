using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public enum ItemType { Melee, Gun, Consumable, Quest }
    public ItemType type;

    [TextArea(3, 5)]
    public string questExplain;

    public GameObject weaponPrefab;
    public float damage;
    public int magSize;
    public int startTotalAmmo;
    public Sprite itemIcon;
}
