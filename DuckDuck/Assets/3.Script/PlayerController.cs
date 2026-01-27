using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // bo hum -- all jakk ddak
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float rollSpeed = 15f;
    public float rollDuration = 0.5f;

    [Header("Visual Settings")]
    public Transform visualChild;

    private Rigidbody _rb;
    private Animator _ani;
    private Camera _mainCamera;
    private Vector3 _moveInput;
    private bool _isRolling = false;

    private static readonly int ANIM_SPEED = Animator.StringToHash("Speed");
    private static readonly int ANIM_ROLL = Animator.StringToHash("Roll");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _ani = visualChild.GetComponent<Animator>(); 
        _mainCamera = Camera.main;

        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
    {
        if (_isRolling) return;

        HandleRotation();
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (_isRolling) return;
        MovePlayer();
    }

    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal"); //hashvalue
        float v = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector3(h, 0, v).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && _moveInput.sqrMagnitude > 0)
        {
            StartCoroutine(RollRoutine());
        }
    }

    private void HandleRotation()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
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

        Vector3 targetVelocity = _moveInput * speed;
        _rb.linearVelocity = new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.z);

        float animValue = _moveInput.sqrMagnitude > 0 ? (isSprinting ? 1.0f : 0.5f) : 0f;
        _ani.SetFloat(ANIM_SPEED, animValue, 0.1f, Time.fixedDeltaTime);
    }

    private IEnumerator RollRoutine()
    {
        _isRolling = true;
        _ani.SetTrigger(ANIM_ROLL);

        Vector3 rollDir = _moveInput;
        if (rollDir == Vector3.zero) rollDir = transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(rollDir);
        _rb.rotation = targetRotation;
        transform.rotation = targetRotation;

        float startTime = Time.time;
        Vector3 originalLocalPos = visualChild.localPosition;

        while (Time.time < startTime + rollDuration)
        {
            _rb.MoveRotation(targetRotation);
            float elapsedTime = (Time.time - startTime) / rollDuration;
            float currentSpeed = Mathf.Lerp(rollSpeed, walkSpeed, elapsedTime);

            float yVel = _rb.linearVelocity.y;
            _rb.linearVelocity = new Vector3(rollDir.x * currentSpeed, yVel, rollDir.z * currentSpeed);

            visualChild.localRotation = Quaternion.Euler(elapsedTime * 360f, 0, 0);
            float yOffset = Mathf.Sin(elapsedTime * Mathf.PI) * 0.5f;
            visualChild.localPosition = new Vector3(originalLocalPos.x, originalLocalPos.y + yOffset, originalLocalPos.z);

            yield return null;
        }
        //reset
        visualChild.localRotation = Quaternion.identity;
        visualChild.localPosition = originalLocalPos;
        _isRolling = false;
    }
}