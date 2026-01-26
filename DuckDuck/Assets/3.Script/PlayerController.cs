using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // bo hum -- alljallddak
public class PlayerController : MonoBehaviour
{
    [Header("MovementSpeed")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float rollSpeed = 12f;
    public float rollDuration = 0.5f;

    [Header("Components")]
    private Rigidbody _rb;
    private Animator _ani;
    private Camera _mainCamera;

    private Vector3 moveInput;
    private bool isRolling = false;

    private static readonly int ANIM_SPEED = Animator.StringToHash("Speed");
    private static readonly int ANIM_ROLL = Animator.StringToHash("Roll"); //hashvalue

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _ani = GetComponentInChildren<Animator>();
        _mainCamera = Camera.main;

        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate; //smooth
    }
    private void Update()
    {
        if (isRolling) return;

        HandleInput();
        HandleRotation();
    }
    private void FixedUpdate()
    {
        if (isRolling) return;
        MovePlayer();
    }
    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(h, 0, v).normalized;

        if(Input.GetKeyDown(KeyCode.Space) && moveInput.sqrMagnitude > 0)
        {
            StartCoroutine(RollRoutine());
        }
    }
    private void HandleRotation()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if(groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 lookDir = targetPoint - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                _rb.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
    private void MovePlayer()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? runSpeed : walkSpeed;

        Vector3 targetVelocity = moveInput * speed; // set rigidbody speed setting
        _rb.linearVelocity = new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.z);

        float animValue = moveInput.sqrMagnitude > 0 ? (isSprinting ? 1.0f : 0.5f) : 0f;
        _ani.SetFloat(ANIM_SPEED, animValue, 0.1f, Time.fixedDeltaTime);
    }
    private IEnumerator RollRoutine()
    {
        isRolling = true;
        _ani.SetTrigger(ANIM_ROLL);

        Vector3 rollDir = moveInput;
        float startTime = Time.time;

        Transform turnturn = _ani.transform;

        Vector3 originalLocalPos = turnturn.localPosition;

        while (Time.time < startTime + rollDuration)
        {
            float yVel = _rb.linearVelocity.y;
            if (yVel > 0) yVel = -2f;
            _rb.linearVelocity = new Vector3(rollDir.x * rollSpeed, yVel, rollDir.z * rollSpeed);

            float elapsedTime = (Time.time - startTime) / rollDuration;
            turnturn.localRotation = Quaternion.Euler(elapsedTime * 360f, 0, 0);

            float yOffset = Mathf.Sin(elapsedTime * Mathf.PI) * 0.7f;
            turnturn.localPosition = new Vector3(originalLocalPos.x, originalLocalPos.y + yOffset, originalLocalPos.z);
            yield return null;
        }
        turnturn.localRotation = Quaternion.identity;
        turnturn.localPosition = originalLocalPos;
        isRolling = false;
    }
}
