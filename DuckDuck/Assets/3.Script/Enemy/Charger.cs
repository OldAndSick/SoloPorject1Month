using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Charger : EnemyBase
{
    [Header("Settings")]
    public Transform player;
    public float detectRange = 20f;
    public float chargeRange = 12f;

    [Header("Speed & Damage")]
    public float walkSpeed = 3.5f;
    public float chargeSpeed = 20f;
    public float chargeDamage = 40f;

    [Header("HP & ITEM")]
    public GameObject dropItemPrefab;

    [Header("Wander Settings")]
    public float walkRadius = 15f;
    public float minWaitTime = 1.5f;
    public float maxWaitTime = 4f;

    private NavMeshAgent agent;
    private bool isCharging = false;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        if (noticeUI != null) noticeUI.SetActive(false);
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        StartCoroutine(WalkRoutine());
    }
    private void Update()
    {
        if (player == null || isCharging || isDead) return;
        float dis = Vector3.Distance(transform.position, player.position);
        // [띠또 마법] FOV 시야 통제 추가!!
        // 거리가 가깝고(dis <= detectRange) + 화면에 보일 때(isCurrentVisible)만 켜라!
        if (noticeUI != null)
        {
            noticeUI.SetActive((dis <= detectRange) && isCurrentVisible);
        }
        if (dis < detectRange && dis > chargeRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else if(dis <= chargeRange)
        {
            StartCoroutine(ChargeRoutine());
        }
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true;

        agent.isStopped = true;

        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        yield return new WaitForSeconds(1.0f); 

        Debug.Log("뺑소니 돌진!!!!");
        agent.isStopped = false;
        agent.speed = chargeSpeed;
        agent.acceleration = 100f; 

        Vector3 targetPos = transform.position + transform.forward * chargeRange;
        agent.SetDestination(targetPos);

        yield return new WaitForSeconds(1.2f); 

        Debug.Log("헉헉... (쿨타임)");
        agent.isStopped = true;
        agent.speed = walkSpeed;
        agent.acceleration = 8f; 

        yield return new WaitForSeconds(2.5f); 

        isCharging = false; 
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (isCharging && agent.speed == chargeSpeed && !isDead)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController pc = other.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.TakeDamage(chargeDamage);
                }
            }
        }
    }
    protected override void Die()
    {
        base.Die();

        if (noticeUI != null) noticeUI.SetActive(false);
        isCharging = false;
        agent.isStopped = true;

        if (dropItemPrefab != null) Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 1.0f);
    }
    private IEnumerator WalkRoutine()
    {
        while (!isDead)
        {
            // 플레이어가 가까이 있거나 돌진 중일 때는 산책 금지!
            bool isDetectingPlayer = (player != null && Vector3.Distance(transform.position, player.position) <= detectRange);
            if (isDetectingPlayer || isCharging)
            {
                yield return null;
                continue;
            }

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
                if (isDead || isCharging) break;
                // 걸어가다가 플레이어가 사거리 안에 들어오면 즉시 산책 중단!
                if (player != null && Vector3.Distance(transform.position, player.position) <= detectRange) break;
                yield return null;
            }

            while (timer < waitTime)
            {
                if (isDead || isCharging) break;
                if (player != null && Vector3.Distance(transform.position, player.position) <= detectRange) break;
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
