using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObstacle : MonoBehaviour
{
    [Header("Cover Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Effects")]
    public GameObject brokenPrefab; //effect

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"[장애물 맞음!] 현재 체력: {currentHealth}");
        if (currentHealth <= 0)
        {
            BreakCover();
        }
    }

    private void BreakCover()
    {
        if (brokenPrefab != null)
        {
            Instantiate(brokenPrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
