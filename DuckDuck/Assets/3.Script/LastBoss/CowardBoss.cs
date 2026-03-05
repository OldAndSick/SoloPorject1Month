using UnityEngine;
using UnityEngine.AI;

public class CowardBoss : MonoBehaviour
{
    public Transform player;
   
    public float retreatDistance = 7f; //�� �Ÿ����� ������ ����

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
        //�÷��̾� �ݴ� ���� ���
        Vector3 dirToPlayer = transform.position - player.position;
        //�ݴ�������� �����Ÿ� ������ ��ǥ ���� ����
        Vector3 retreatTarget = transform.position + dirToPlayer.normalized * 5f;

        //������ �ش� �������� �̵�
        agent.SetDestination(retreatTarget);
    }
    void StopAndAttack()
    {
        agent.ResetPath(); // �̵� ����
        transform.LookAt(player); // �÷��̾� �ֽ�

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
        }
    }
}
