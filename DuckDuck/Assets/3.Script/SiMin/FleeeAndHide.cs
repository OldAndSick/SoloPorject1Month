using UnityEngine;
using UnityEngine.AI;

public class FleeeAndHide : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float detectRange = 15f;
    public float hideDistance = 2f;
    public LayerMask HideObstacleLayer;

    private NavMeshAgent agent;
    private float thinkTimer = 0f;
    private float thinkInterval = 0.5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectRange) return;
        thinkTimer -= Time.deltaTime;
        if (thinkTimer <= 0f)
        {
            ThinkAndMove();
            thinkTimer = thinkInterval;
        }
    }

    private void ThinkAndMove()
    {
        Collider[] obstacles = Physics.OverlapSphere(transform.position, 10f, HideObstacleLayer);

        if (obstacles.Length > 0)
        {
            HideBehind(obstacles[0].transform);
        }
        else
        {
            FleeFromPlayer();
        }
    }
    void HideBehind(Transform obstacle)
    {
        Vector3 dir = (obstacle.position - player.position).normalized;
        Vector3 hidePos = obstacle.position + dir * hideDistance;

        if (NavMesh.SamplePosition(hidePos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FleeFromPlayer()
    {
        Vector3 runDir = (transform.position - player.position).normalized;
        Vector3 targetPos = transform.position + runDir * 5f;
        agent.SetDestination(targetPos);
    }
}
