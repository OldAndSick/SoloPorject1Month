using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("Box Settings")]
    public float health = 30f; 

    [Header("Effects")]
    public GameObject brokenEffectPrefab;

    [Header("Loot Drop (Random)")]
    public GameObject[] dropItemPrefabs;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        if (brokenEffectPrefab != null)
        {
            Instantiate(brokenEffectPrefab, transform.position, transform.rotation);
        }
        if (dropItemPrefabs != null && dropItemPrefabs.Length > 0)//random
        {
            int randomIndex = UnityEngine.Random.Range(0, dropItemPrefabs.Length);
            if (dropItemPrefabs[randomIndex] != null)
            {
                Instantiate(dropItemPrefabs[randomIndex], transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }
}
