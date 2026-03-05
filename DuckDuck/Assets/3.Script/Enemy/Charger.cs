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
    public AudioClip actionSound;

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
        agent.angularSpeed = 120f; // (기본값은 보통 엄청 빨라서 뚝뚝 끊깁니다)

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
        if(actionSound != null)
        {
            myAudio.PlayOneShot(actionSound);
        }
        float windupTime = 1.0f; // 기 모으는 시간
        float timer = 0f;

        while (timer < windupTime)
        {
            if (player == null || isDead) yield break;

            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f) // 안전빵 체크
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                // Slerp: 현재 각도에서 목표 각도까지 스르륵~ 회전시킵니다! (뒤의 숫자가 클수록 휙 돕니다)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
            }

            timer += Time.deltaTime;
            yield return null; // 1프레임 대기
        }

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
