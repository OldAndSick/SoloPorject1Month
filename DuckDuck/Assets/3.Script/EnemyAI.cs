using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Setting")]
    public float detectRange = 7f;
    public Transform player;
    public GameObject noticeUI;

    [Header("Combat")]
    public float health = 100f;
    public float shootingRange = 6f;
    public float stopDistance = 4f;
    public float fireRate = 1.0f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Slider enemyHPBar;

    [Header("Loot set")]
    public GameObject dropItemPrefab;

    private float fireTimer;
    private NavMeshAgent agent;
    private bool isChasing = false;
    private bool isDead = false;
    private Renderer[] renderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
        renderers = GetComponentsInChildren<Renderer>();

        if(enemyHPBar != null)
        {
            enemyHPBar.value = 1f;
        }
        if (noticeUI != null)
        {
            noticeUI.SetActive(false);
        }
    }
    private void Update()
    {
        if (player == null || isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if(distance <= detectRange)
        {
            if(!isChasing)
            {
                StartChase();
            }
            agent.SetDestination(player.position);

            if(distance <= shootingRange)
            {
                Vector3 lookDir = player.position - transform.position;
                lookDir.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);

                fireTimer -= Time.deltaTime;
                if(fireTimer <= 0)
                {
                    Fire();
                    fireTimer = fireRate;
                }
            }
        }
        else if(isChasing && distance > detectRange + 2f)
        {
            StopChase();
        }
    }

    private void StartChase()
    {
        isChasing = true;
        if(noticeUI != null)
        {
            noticeUI.SetActive(true);
        }
        agent.speed = 4f;
    }
    private void StopChase()
    {
        isChasing = false;
        if(noticeUI != null)
        {
            noticeUI.SetActive(false);
        }
        agent.speed = 2f;
    }
    private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector3 targetCenter = player.position + Vector3.up * 1f;
        Vector3 fireDir = (targetCenter - firePoint.position).normalized;
        bullet.transform.forward = fireDir;

        Debug.Log("shoot");
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, 100f);
        if (enemyHPBar != null)
        {
            enemyHPBar.value = health / 100f;
        }
        StartCoroutine(HitFlashRoutine());
        if(health <= 0)
        {
            Die();
        }
    }
    private IEnumerator HitFlashRoutine()
    {
        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.material.color = Color.red;

            if (r.material.HasProperty("_EmissionColor"))
            {
                r.material.EnableKeyword("_EMISSION");
                r.material.SetColor("_EmissionColor", Color.red * 2f);
            }
        }
        yield return new WaitForSeconds(0.3f);

        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.material.color = Color.white; 
            if (r.material.HasProperty("_EmissionColor"))
            {
                r.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        if(dropItemPrefab != null)
        {
            GameObject drop = Instantiate(dropItemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        agent.isStopped = true;
        noticeUI.SetActive(false);
        Destroy(gameObject, 1.0f);
    }
}
