using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowStaminaUI : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(-1.5f, 1f, 0);

    private void LateUpdate()
    {
        transform.position = player.position + offset;
        transform.rotation = Camera.main.transform.rotation;
    }
}
