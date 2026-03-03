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

    public float healAmount = 30f;
    public float useTime = 2.0f;
    [TextArea(3, 5)]
    public string questExplain;

    public GameObject weaponPrefab;
    public float damage;
    public int magSize;
    public int startTotalAmmo;
    public Sprite itemIcon;

    [Header("Weapon Stats")]
    public float gunDamage;
    public float gunSpeed;
    public float fireRate;
    public float gunSpread;

    [Header("Runtime Stats")]
    public int currentMagCount;   // 현재 이 총의 탄창에 들어있는 탄 수
    public int currentTotalAmmo;  // 현재 이 총이 가지고 있는 예비 탄 수
}
