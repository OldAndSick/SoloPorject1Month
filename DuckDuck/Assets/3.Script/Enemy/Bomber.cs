using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bomber : EnemyBase
{
    [Header("Bomber Settings")]
    public Transform player;
    public float detectRange = 15f; // 발견 거리
    public float throwRange = 8f;   // 폭탄 던지는 거리 (이 사거리 안에 오면 멈춤!)

    [Header("Throw Settings")]
    public GameObject bombPrefab;   // 아까 만든 Bomb 프리팹
    public Transform throwPoint;    // 폭탄이 날아갈 손 위치
    public float throwForce = 10f;  // 앞으로 던지는 힘
    public float upwardForce = 5f;  // 위로 던지는 힘 (곡사포)
    public float throwCooldown = 3.5f; // 던지는 쿨타임

    [Header("Wander Settings")]
    public float walkRadius = 10f;
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    [Header("Item Drop")]
    public GameObject[] dropItemPrefabs;

    private NavMeshAgent agent;
    private Animator anim;
    private bool isChasing = false;
    private float lastThrowTime = 0f;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (noticeUI != null) noticeUI.SetActive(false);
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        StartCoroutine(WanderRoutine());
    }

    protected override void Update()
    {
        base.Update();
        if (player == null || isDead) return;

        // 애니메이션 연결 (Float 스피드)
        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        float dis = Vector3.Distance(transform.position, player.position);

        // 시야 느낌표 UI (FOV 연동)
        if (noticeUI != null) noticeUI.SetActive((dis <= detectRange) && isCurrentVisible);

        // 1. 발견하면 추격 시작!
        if (dis <= detectRange && !isChasing)
        {
            isChasing = true;
            agent.speed = runSpeed;
        }

        // 2. 추격 중일 때의 행동 로직
        if (isChasing)
        {
            if (dis > throwRange)
            {
                // 사거리 밖이면 달려가기!
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                // 사거리 안으로 들어오면 멈춰서 폭탄 투척!
                agent.isStopped = true;

                // 플레이어 쪽을 쳐다봅니다
                Vector3 lookDir = (player.position - transform.position).normalized;
                lookDir.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

                // 쿨타임 체크 후 투척!
                if (Time.time >= lastThrowTime + throwCooldown)
                {
                    ThrowBomb();
                }
            }
        }
    }

    private void ThrowBomb()
    {
        lastThrowTime = Time.time;
        if (anim != null) anim.SetTrigger("Attack");

        if (bombPrefab != null && throwPoint != null)
        {
            GameObject bomb = Instantiate(bombPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody bombRb = bomb.GetComponent<Rigidbody>();

            if (bombRb != null)
            {
                // [띠또 마법 1] 플레이어의 현재 위치가 아니라, '플레이어가 바라보는 방향 2.5m 앞'을 조준합니다!!
                Vector3 predictPos = player.position + (player.forward * 1.5f);

                Vector3 throwDir = (predictPos - throwPoint.position).normalized;

                Vector3 force = (throwDir * throwForce) + (Vector3.up * upwardForce);
                bombRb.AddForce(force, ForceMode.Impulse);
                // ==========================================
            }
        }
    }

    protected override void Die()
    {
        base.Die();
        if (noticeUI != null) noticeUI.SetActive(false);
        agent.isStopped = true;
        if (anim != null) anim.SetTrigger("Death");
        if (dropItemPrefabs != null && dropItemPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, dropItemPrefabs.Length);
            if (dropItemPrefabs[randomIndex] != null)
            {
                Instantiate(dropItemPrefabs[randomIndex], transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
        Destroy(gameObject, 3.0f);
    }

    private IEnumerator WanderRoutine()
    {
        while (!isDead)
        {
            if (isChasing)
            {
                yield return null;
                continue;
            }

            agent.speed = walkSpeed;
            agent.stoppingDistance = 0f;
            Vector3 randomPos = transform.position + Random.insideUnitSphere * walkRadius;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
                agent.SetDestination(hit.position);

            float waitTime = Random.Range(2f, 5f);
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