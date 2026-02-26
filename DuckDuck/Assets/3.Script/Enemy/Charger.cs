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
    public GameObject noticeUI;

    [Header("Speed & Damage")]
    public float walkSpeed = 3.5f;
    public float chargeSpeed = 20f;
    public float chargeDamage = 40f;

    [Header("HP & ITEM")]
    public GameObject dropItemPrefab;

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
    }
    private void Update()
    {
        if (player == null || isCharging || isDead) return;
        float dis = Vector3.Distance(transform.position, player.position);
        if (dis <= detectRange)
        {
            if (noticeUI != null) noticeUI.SetActive(true);
        }
        else
        {
            if (noticeUI != null) noticeUI.SetActive(false);
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
}
