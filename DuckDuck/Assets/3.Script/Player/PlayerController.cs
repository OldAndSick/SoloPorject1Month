using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Stamina System")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegen = 15f;
    public float runStamina = 20f;
    public float rollStamina = 30f;
    public float regenDelay = 1.5f;
    
    [Header("StaminaUI")]
    public Image staminaRing;
    public CanvasGroup uiGroup;

    [Header("Combat Settings")]
    public float attackRange = 2f;
    public float attackDamage = 30f;
    public LayerMask enemyLayer;
    public GameObject slashVFXPrefab;
    public Transform attackPoint;   

    private float regenTimer;
    private Rigidbody _rb;
    private Animator _ani;
    private Camera _mainCamera;
    private Vector3 _moveInput;
    private bool _isRolling = false;

    private static readonly int ANIM_SPEED = Animator.StringToHash("Speed");
    private static readonly int ANIM_ROLL = Animator.StringToHash("Roll");

    private void Start()
    {
        currentStamina = maxStamina;
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _ani = GetComponent<Animator>();
        _mainCamera = Camera.main;
        currentStamina = maxStamina;

        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
    {
        if (_isRolling) return;

        HandleRotation();
        HandleInput();
        HandleCombat();
        UpdateUI();
    }

    private void FixedUpdate()
    {
        if (_isRolling) return;
        MovePlayer();
        HandleStamina();
    }

    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal"); //hashvalue
        float v = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector3(h, 0, v).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && _moveInput.sqrMagnitude > 0 && currentStamina >= rollStamina)
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
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && _moveInput.sqrMagnitude > 0;
        float speed = isSprinting ? runSpeed : walkSpeed;

        Vector3 targetVelocity = _moveInput * speed;
        _rb.linearVelocity = new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.z);

        float animValue = _moveInput.sqrMagnitude > 0 ? (isSprinting ? 1.0f : 0.5f) : 0f;
        _ani.SetFloat(ANIM_SPEED, animValue, 0.1f, Time.fixedDeltaTime);
    }

    private IEnumerator RollRoutine()
    {
        _isRolling = true;
        currentStamina -= rollStamina;
        regenTimer = regenDelay;
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
            float yOffset = Mathf.Sin(elapsedTime * Mathf.PI) * 5f;
            visualChild.localPosition = new Vector3(originalLocalPos.x, originalLocalPos.y + yOffset, originalLocalPos.z);

            yield return null;
        }
        //reset
        visualChild.localRotation = Quaternion.identity;
        visualChild.localPosition = originalLocalPos;
        _isRolling = false;
    }

    private void HandleStamina()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && _moveInput.sqrMagnitude > 0 && !_isRolling;
        if (isSprinting && currentStamina > 0)
        {
            currentStamina -= runStamina * Time.fixedDeltaTime;
            regenTimer = regenDelay;
        }
        else if(regenTimer >0)
        {
            regenTimer -= Time.fixedDeltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegen * Time.fixedDeltaTime;
            }
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }
    private void UpdateUI()
    {
        if(staminaRing == null || uiGroup == null)
        {
            return;
        }

        float ratio = currentStamina / maxStamina;
        staminaRing.fillAmount = ratio;

        if (ratio > 0.7f) staminaRing.color = new Color(0.2f, 1f, 0.2f); 
        else if (ratio > 0.3f) staminaRing.color = Color.yellow;
        else staminaRing.color = Color.red;

        float targetAlpha = (currentStamina < maxStamina) ? 1f : 0f;
        uiGroup.alpha = Mathf.Lerp(uiGroup.alpha, targetAlpha, Time.deltaTime * 5f);
    }
    private void HandleCombat()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PerformMeleeAttack();
        }
    }

    private void PerformMeleeAttack()
    {
        if(slashVFXPrefab != null && attackPoint != null)
        {
            Instantiate(slashVFXPrefab, attackPoint.position, attackPoint.rotation);
        }
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        Debug.Log($"공격 범위 내 감지된 콜라이더 수: {hitEnemies.Length}");
        foreach (Collider enemy in hitEnemies)
        {
            if(enemy.TryGetComponent(out EnemyAI enemyAI))
            {
                enemyAI.TakeDamage(attackDamage);
                Debug.Log($"{enemy.name}에게 {attackDamage}의 데미지를 입힘!");
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}