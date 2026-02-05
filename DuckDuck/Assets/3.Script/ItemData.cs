using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public enum ItemType { Melee, Gun, Consumable }
    public ItemType type;
    public GameObject weaponPrefab;
    public float damage;
    public int magSize;
    public int startTotalAmmo;
    public Sprite itemIcon;
}
