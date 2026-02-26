using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;
 
    [Header("Base UI")]
    public Slider enemyHPBar;

    [Header("Base Effects")]
    public SkinnedMeshRenderer[] meshes;
    private Color[] originalColor;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        if(meshes != null && meshes.Length >0)
        {
            originalColor = new Color[meshes.Length];
            for(int i =0; i< meshes.Length; i++)
            {
                originalColor[i] = meshes[i].material.color;
            }
        }
    }
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} 피격!! 남은 체력: {currentHealth}");

        if (enemyHPBar != null)
        {
            enemyHPBar.value = currentHealth / maxHealth;
        }

        StartCoroutine(HitFlashRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log($"{gameObject.name} 사망!!");
    }

    protected IEnumerator HitFlashRoutine()
    {
        if (meshes == null || meshes.Length == 0) yield break;
        for (int i = 0; i < meshes.Length; i++)
            meshes[i].material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshes.Length; i++)
            meshes[i].material.color = originalColor[i];
    }
}
