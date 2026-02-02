using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class Pettest : MonoBehaviour
{
    public float derectionRange = 10f; //적 감지 범위
    public LayerMask enemyLayer; //적..
    public LineRenderer laserLine; //라인렌더러 연결
    public Transform mouthPos;

    private Transform target;

    private void Start()
    {
        laserLine.positionCount = 2;
        laserLine.useWorldSpace = true;
    }

    private void Update()
    {
        FindNearesEnemy();
        if (target != null)
        {
            DrawLaser();
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookrotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, Time.deltaTime * 5f);
        }
        else
        {
            laserLine.enabled = false;
        }
    }

    void FindNearesEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, derectionRange, enemyLayer);
        float shortest = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider enemy in enemies)
        {
            float distancetoenemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distancetoenemy < shortest)
            {
                shortest = distancetoenemy;
                nearestEnemy = enemy.transform;
            }
        }
        target = nearestEnemy;
    }
    void DrawLaser()
    {
        laserLine.enabled = true;
        laserLine.SetPosition(0, mouthPos.position);

        laserLine.SetPosition(1, target.position);

        float offset = Time.time * -3f;
        laserLine.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
