using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target set")]
    public Transform playerTransform;

    [Header("Camera move")]
    public float targettingSpeed = 0.125f;
    public float mouseSee = 0.2f;
    public float mouseDistance = 3f;

    [Header("Position Offset")]
    public Vector3 cameraOffset = new Vector3(0, 10, -5);

    private Vector3 velo = Vector3.zero;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    private void LateUpdate() //after player move
    {
        if(playerTransform == null)
        {
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, playerTransform.position);

        if(groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(rayDistance);
            Vector3 targetOffset = (mouseWorldPos - playerTransform.position) * mouseSee;

            targetOffset = Vector3.ClampMagnitude(targetOffset, mouseDistance); // clamp - limit

            Vector3 targetPos = playerTransform.position + cameraOffset + targetOffset;

            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velo, targettingSpeed);
        }
    }
}
