using UnityEngine;
using UnityEngine.AI;

public class FleeeAndHide : MonoBehaviour
{
    public Transform player;
    public float hideDistance = 2f;
    private NavMeshAgent agent;

    private void Start() => agent = GetComponent<NavMeshAgent>();

    private void Update()
    {
        Collider[] obstacles = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask("Cover"));

        if(obstacles.Length>0)
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

        if(NavMesh.SamplePosition(hidePos,out NavMeshHit hit,2f,NavMesh.AllAreas))
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
