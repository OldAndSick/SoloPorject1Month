using UnityEngine;
using UnityEngine.AI;

public class CowardBoss : MonoBehaviour
{
    public Transform player;
   
    public float retreatDistance = 7f; //이 거리보다 가까우면 도망

    private NavMeshAgent agent;
    private float lastAttackTime;
    private float attackCooldown = 2.5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < retreatDistance)
        {
            Retreat();
        }
        else
        {
            StopAndAttack();
        }
    }

    void Retreat()
    {
        //플레이어 반대 방향 계산
        Vector3 dirToPlayer = transform.position - player.position;
        //반대방향으로 일정거리 떨어진 목표 지점 설정
        Vector3 retreatTarget = transform.position + dirToPlayer.normalized * 5f;

        //위에서 해당 지점으로 이동
        agent.SetDestination(retreatTarget);
    }
    void StopAndAttack()
    {
        agent.ResetPath(); // 이동 멈춤
        transform.LookAt(player); // 플레이어 주시

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
        }
    }
}
