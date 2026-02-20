using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(1.5f, 2f, -1f);
    public float smoothTime = 0.3f;

    [Header("Wall Check")]
    public float petRaduis = 0.5f;
    public LayerMask wallLayer;

    private Vector3 _velocity = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 targetPos = player.position + (player.forward * offset.z) + (player.right * offset.x) + (Vector3.up * offset.y);

        Vector3 dir = (targetPos - player.position).normalized;
        float distance = Vector3.Distance(player.position, targetPos);
        Vector3 finalTargetPos = targetPos;
        if(Physics.SphereCast(player.position, petRaduis, dir, out RaycastHit hit, distance, wallLayer))
        {
            finalTargetPos = hit.point + (hit.normal * petRaduis);
        }
        transform.position = Vector3.SmoothDamp(transform.position, finalTargetPos, ref _velocity, smoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, Time.deltaTime * 2f);
    }
}
