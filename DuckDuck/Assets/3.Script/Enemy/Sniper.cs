using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sniper : EnemyBase
{
    [Header("Sniper Settings")]
    public Transform player;
    public float detectRange = 25f;
    public float shootingRange = 20f;
    public float escapeRange = 7f;     // 플레이어가 너무 붙으면 튑니다!

    [Header("Combat Settings")]
    public float sniperDamage = 50f;
    public float fireRate = 4.0f;
    public float aimTime = 1.5f;
    public float aimOffset = 0.5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public LineRenderer laserLine;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float walkRadius = 10f;
    
    [Header("Loot Drop")]
    public GameObject dropItemPrefab;

    private NavMeshAgent agent;
    private Animator anim;
    private float lastFireTime;
    private bool isChasing = false;
    private bool isAiming = false;

    protected override void Start() 
    {
        base.Start(); // 조상님의 체력 세팅 등을 실행합니다!
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if (laserLine != null) laserLine.enabled = false;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(WanderRoutine());
    }

    protected override void Update() 
    {
        base.Update(); 
        if (isDead || player == null) return; 

        
        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        // 2. FOV에 따른 시야 UI 연동 (조상님의 noticeUI 활용!)
        if (noticeUI != null)
        {
            noticeUI.SetActive(isCurrentVisible && isChasing);
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // 3. 거리별 행동 트리거
        if (!isChasing && distance <= detectRange) isChasing = true;

        if (isChasing)
        {
            HandleSniperBehavior(distance);
        }
    }

    private void HandleSniperBehavior(float distance)
    {
        if (distance < escapeRange)
        {
            isAiming = false;
            if (laserLine != null) laserLine.enabled = false;

            agent.isStopped = false;
            agent.speed = runSpeed; // 뛸 때 느낌 아니까!
            Vector3 runDir = (transform.position - player.position).normalized;
            agent.SetDestination(transform.position + runDir * 5f);
        }
        else if (distance <= shootingRange) // 여기서 한 방!
        {
            agent.isStopped = true;
            LookAtPlayer();

            if (Time.time >= lastFireTime + fireRate && !isAiming)
            {
                StartCoroutine(AimAndShoot());
            }
        }
        else // 좀 더 가까이...
        {
            agent.isStopped = false;
            agent.speed = runSpeed;
            agent.SetDestination(player.position);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }

    private IEnumerator AimAndShoot()
    {
        isAiming = true;
        if (laserLine != null) laserLine.enabled = true;

        float timer = 0;
        while (timer < aimTime)
        {
            if (isDead) yield break;

            if (laserLine != null)
            {
                laserLine.SetPosition(0, firePoint.position);
                laserLine.SetPosition(1, player.position + Vector3.up * aimOffset);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        Fire();
    }

    private void Fire()
    {
        if (isDead) return; 
        if (laserLine != null) laserLine.enabled = false;
        Vector3 targetCenter = player.position + Vector3.up * aimOffset;
        firePoint.LookAt(targetCenter);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = sniperDamage; // 저격수의 데미지를 총알에게 전달!!
        }
        lastFireTime = Time.time;
        isAiming = false;
    }

    protected override void Die() // 조상님의 Die를 저격수 스타일로 확장!
    {
        base.Die(); //
        if (laserLine != null) laserLine.enabled = false;
        if (anim != null) anim.SetTrigger("Death");

        // 가챠 템 드롭!
        if (dropItemPrefab != null)
        {
            Instantiate(dropItemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        agent.isStopped = true;
        Destroy(gameObject, 3f);
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (isDead) yield break;

            if (!isChasing && !isAiming)
            {
                Vector3 randomPos = transform.position + Random.insideUnitSphere * walkRadius;

                // NavMesh 상의 실제 갈 수 있는 위치인지 확인합니다.
                if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
                {
                    agent.isStopped = false;
                    agent.speed = walkSpeed; // 걷기 속도로 천천히!
                    agent.SetDestination(hit.position);
                }
            }

            // 3. 목적지에 도착할 때까지 대기하거나, 랜덤한 시간만큼 멍 때립니다.
            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }
}