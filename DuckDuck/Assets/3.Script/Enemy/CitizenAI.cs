using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CitizenAI : EnemyBase
{
    [Header("Citizen Settings")]
    public Transform player;
    public float panicRange = 10f;
    public float safeRange = 20f;
    public float hideDistance = 2f;
    public LayerMask hideObstacleLayer;

    [Header("walk Settings(san check)")]
    public float walkRadius = 15f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    [Header("Speed Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 6f;

    private NavMeshAgent agent;
    private Animator anim;
    private bool isPanick = false;
    private float thinkTimer = 0f;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        StartCoroutine(WanderRoutine());
    }

    private void Update()
    {
        if (isDead) return;

        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= panicRange && !isPanick)
        {
            Panic();
        }
        if (isPanick)
        {
            if (distanceToPlayer > safeRange)
            {
                CalmDown();
            }
            else
            {
                ThinkAndMove();
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        if (isDead) return;

        base.TakeDamage(damage);

        if (!isPanick)
        {
            Panic();
        }
    }

    private void Panic()
    {
        isPanick = true;
        StopAllCoroutines();
        agent.speed = runSpeed;
    }

    private IEnumerator WanderRoutine()
    {
        agent.speed = walkSpeed;

        while (!isPanick)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * walkRadius;
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            while (agent.pathPending || agent.remainingDistance > 0.5f)
            {
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
    }

    private void ThinkAndMove()
    {
        thinkTimer -= Time.deltaTime;
        if (thinkTimer > 0f) return;
        thinkTimer = 0.5f;

        Collider[] obstacles = Physics.OverlapSphere(transform.position, 15f, hideObstacleLayer);

        if (obstacles.Length > 0)
        {
            HideBehind(obstacles[0].transform);
        }
        else
        {
            FleeFromPlayer();
        }
    }

    private void HideBehind(Transform obstacle)
    {
        Vector3 dir = (obstacle.position - player.position).normalized;
        Vector3 hidePos = obstacle.position + dir * hideDistance;

        if (NavMesh.SamplePosition(hidePos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void FleeFromPlayer()
    {
        Vector3 runDir = (transform.position - player.position).normalized;
        Vector3 targetPos = transform.position + runDir * 10f;
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    protected override void Die()
    {
        base.Die();
        agent.isStopped = true;
        anim.SetTrigger("Death");
        Destroy(gameObject, 3f);
    }
    private void CalmDown()
    {
        isPanick = false;
        StartCoroutine(WanderRoutine());
    }
}