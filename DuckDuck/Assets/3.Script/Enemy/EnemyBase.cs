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
    public GameObject noticeUI; 
    public bool isCurrentVisible = false;

    [Header("Base Effects")]
    public SkinnedMeshRenderer[] meshes;
    private Color[] originalColor;
    private Camera mainCam;

    [Header("Sound Settings")]
    public AudioSource myAudio;    // 몸통에 달린 스피커
    public AudioClip hitSound;     // 맞았을 때 "윽!"
    public AudioClip dieSound;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        mainCam = Camera.main;

        if (enemyHPBar != null)
        {
            enemyHPBar.value = 1f;
        }
        if (meshes != null && meshes.Length >0)
        {
            originalColor = new Color[meshes.Length];
            for(int i =0; i< meshes.Length; i++)
            {
                originalColor[i] = meshes[i].material.color;
            }
        }
    }
    protected virtual void Update()
    {
        if (isDead)
        {
            if (enemyHPBar != null) enemyHPBar.gameObject.SetActive(false);
            return;
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
        if (myAudio != null && hitSound != null)
        {
            myAudio.PlayOneShot(hitSound);
        }
    }
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log($"{gameObject.name} 사망!!");
        if (myAudio != null && dieSound != null)
        {
            myAudio.PlayOneShot(dieSound);
        }
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
    protected void CheckUIVisibility()
    {
        if (enemyHPBar == null || mainCam == null) return;

        Vector3 headPos = transform.position + Vector3.up * 1.5f;
        Vector3 dirToCam = mainCam.transform.position - headPos;
        float distToCam = dirToCam.magnitude;

        Vector3 viewPos = mainCam.WorldToViewportPoint(headPos);
        if (viewPos.z < 0 || viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        {
            enemyHPBar.gameObject.SetActive(false);
            return;
        }

        if (Physics.Raycast(headPos, dirToCam.normalized, out RaycastHit hit, distToCam))
        {
            if (!hit.collider.CompareTag("MainCamera") && !hit.collider.CompareTag("Player"))
            {
                enemyHPBar.gameObject.SetActive(false);
                return;
            }
        }
        enemyHPBar.gameObject.SetActive(true);
    }
}
