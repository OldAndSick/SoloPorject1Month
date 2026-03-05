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
    [Header("Enemy Ammo")]
    public int enemyMagSize = 30;
    private int currentEnemyMag;
    private bool isEnemyReloading = false;
    [Header("Walking Settings")]
    public float walkRadius = 10f;     
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float walkSpeed = 2f;

    [Header("Sound Settings")]
    public AudioSource myAudio;    // 몸통에 달린 스피커
    public AudioClip hitSound;     // 맞았을 때 "윽!"
    public AudioClip dieSound;     // 죽을 때 "꽥!"
    public AudioClip actionSound;

    private float fireTimer;
    private NavMeshAgent agent;
    private bool isChasing = false;
    private bool isDead = false;
    private Renderer[] renderers;
    public bool isCurrentVisible = false;
    private Animator anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponentInChildren<Animator>();
        if(enemyHPBar != null)
        {
            enemyHPBar.value = 1f;
        }
        if (noticeUI != null)
        {
            noticeUI.SetActive(false);
        }
        currentEnemyMag = enemyMagSize;
    }
    private void Start()
    {
        if(renderers != null)
        {
            foreach(Renderer r in renderers)
            {
                r.enabled = false;
            }
        }
        StartCoroutine(WalkRoutine());
    }
    private void Update()
    {
        if (player == null || isDead) return;
        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
        if (noticeUI != null)
        {
            bool shouldBeVisible = isCurrentVisible && isChasing;
            noticeUI.SetActive(isCurrentVisible && isChasing);
        }
        float distance = Vector3.Distance(transform.position, player.position);
        if (!isChasing && distance <= detectRange)
        {
            StartChase();
        }

        if (isChasing)
        {
            agent.SetDestination(player.position);

            if (distance <= shootingRange)
            {
                Vector3 lookDir = player.position - transform.position;
                lookDir.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);

                fireTimer -= Time.deltaTime;
                if (fireTimer <= 0)
                {
                    Fire();
                    fireTimer = fireRate;
                }
            }

            if (distance > 15f)
            {
                StopChase();
            }
        }
    }
    private void StartChase()
    {
        isChasing = true;
        agent.speed = 4f;
        agent.stoppingDistance = stopDistance;
    }
    private void StopChase()
    {
        isChasing = false;
        agent.speed = 2f;
    }
    private void Fire()
    {
        if (isEnemyReloading || isDead) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector3 targetCenter = player.position + Vector3.up * 1f;
        Vector3 fireDir = (targetCenter - firePoint.position).normalized;
        bullet.transform.forward = fireDir;

        Debug.Log("shoot");

        currentEnemyMag--;
        if(currentEnemyMag <=0)
        {
            StartCoroutine(EnemyReloadRoutine());
        }
        if (actionSound != null)
        {
            myAudio.PlayOneShot(actionSound);
        }
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, 100f);
        
        if(!isDead)
        {
            isChasing = true;
            if(player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
        }
        if(player != null && !isDead)
        {
            StartChase();
        }
        if (enemyHPBar != null)
        {
            enemyHPBar.value = health / 100f;
        }
        StartCoroutine(HitFlashRoutine());

        if(health <= 0)
        {
            Die();
        }
        if (myAudio != null && hitSound != null)
        {
            myAudio.PlayOneShot(hitSound);
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
    IEnumerator EnemyReloadRoutine()
    {
        isEnemyReloading = true;
        Debug.Log("적 장전중");
        yield return new WaitForSeconds(3.0f); 

        currentEnemyMag = enemyMagSize; 
        isEnemyReloading = false;
    }
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        if (anim != null) anim.SetTrigger("Death");
        if (dropItemPrefab != null)
        {
            GameObject drop = Instantiate(dropItemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        agent.isStopped = true;
        noticeUI.SetActive(false);
        if (myAudio != null && dieSound != null)
        {
            myAudio.PlayOneShot(dieSound);
        }
        Destroy(gameObject, 2.0f);

    }
    public void SetUIActive(bool isVisible)
    {
        isCurrentVisible = isVisible; // FOV가 알려준 시야 상태 저장!
        if (enemyHPBar != null) enemyHPBar.gameObject.SetActive(isVisible);
    }
    private IEnumerator WalkRoutine()
    {
        while (!isDead)
        {
            if (isChasing || isEnemyReloading)
            {
                yield return null;
                continue;
            }
            agent.stoppingDistance = 0f;
            agent.speed = walkSpeed;
            Vector3 randomPos = transform.position + UnityEngine.Random.insideUnitSphere * walkRadius;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            float timer = 0;

            while (agent.pathPending || agent.remainingDistance > 0.5f)
            {
                if (isChasing || isDead) break;
                yield return null;
            }

            while (timer < waitTime)
            {
                if (isChasing || isDead) break;
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
