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
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;
    public GameObject slashVFXPrefab;
    public Transform attackPoint;
    [Header("Player Health UI")]
    public float maxHP = 100f;
    public float currentHP;
    public Slider playerHPUI;
    public Slider playerHeadBar;
    [Header("Inventory Settings")]
    public ItemData currentWeapon;
    public GameObject weaponHolder;
    public GameObject playerBulletPrefab;
    [Header("Gun Settings")]
    public float bulletSpread = 2.0f;
    private GameObject currentWeaponModel;

    [Header("Ammo Settings")]
    public int currentMag;
    public int totalAmmo;
    public bool isReloading = false;
    public Text ammoUI;

    private float lastAtackTime;
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
        currentHP = maxHP;
        UpdateHPUI();
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
        if (_isRolling || isReloading) return;

        HandleRotation();
        HandleInput();
        HandleCombat();
        UpdateUI();

        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadRoutine());
        }
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
    
    private void PerformMeleeAttack()
    {
        if (Time.time < lastAtackTime + attackCooldown) return;

        lastAtackTime = Time.time;
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
    
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"아야! 현재 체력: {currentHP}");
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHPUI();
        if(currentHP <= 0)
        {
            Die();
        }
    }
    private void UpdateHPUI()
    {
        float ratio = currentHP / maxHP;
        if (playerHPUI != null) playerHPUI.value = ratio;
        if (playerHeadBar != null) playerHeadBar.value = ratio;
    }    
    private void Die()
    {
        Debug.Log("die");
    }
    public void AcquireItem(ItemData data)
    {
        if (data == null) return;

        currentWeapon = data;
        UpdateWeaponModel(data.weaponPrefab);

        if(data.type == ItemData.ItemType.Gun)
        {
            currentMag = data.magSize;
            totalAmmo = data.startTotalAmmo;
            UpdateAmmoUI();
            Debug.Log("장착");
        }
    }
    private void HandleCombat()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(currentWeapon != null && currentWeapon.type == ItemData.ItemType.Gun)
            {
                Shoot();
            }
            else
            {
                PerformMeleeAttack();
            }
        }
    }

    private void Shoot()
    {
        if(currentMag <= 0)
        {
            Debug.Log("탄없");
            return;
        }

        if (Time.time < lastAtackTime + attackCooldown) return;
        currentMag--;
        UpdateAmmoUI();

        if (playerBulletPrefab == null) Debug.LogError("playerBulletPrefab이 비어있다 이놈아!");
        if (attackPoint == null) Debug.LogError("attackPoint가 비어있다 이놈아!");

        if (playerBulletPrefab != null && attackPoint != null)
        {
            float spreadX = Random.Range(-bulletSpread, bulletSpread);
            float spreadY = Random.Range(-bulletSpread, bulletSpread);
            Quaternion spreadRotation = Quaternion.Euler(spreadX, spreadY, 0);
            GameObject bullet = Instantiate(playerBulletPrefab, attackPoint.position, attackPoint.rotation * spreadRotation);

            Debug.Log("shoot");
        }
        lastAtackTime = Time.time;
    }
    IEnumerator ReloadRoutine()
    {
        if (totalAmmo <= 0 || currentMag == currentWeapon.magSize) yield break;

        isReloading = true;
        Debug.Log("장전중...");
        yield return new WaitForSeconds(2.0f); 

        int needAmmo = currentWeapon.magSize - currentMag;
        int reloadAmount = Mathf.Min(totalAmmo, needAmmo);

        totalAmmo -= reloadAmount;
        currentMag += reloadAmount;

        isReloading = false;
        UpdateAmmoUI();
        Debug.Log("재장전 완료");
    }

    private void UpdateAmmoUI()
    {
        if (ammoUI != null) ammoUI.text = $"{currentMag} / {totalAmmo}";
    }
    private void UpdateWeaponModel(GameObject prefab)
    {
        if(currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
        if(prefab != null && weaponHolder != null)
        {
            currentWeaponModel = Instantiate(prefab, weaponHolder.transform);
            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation = Quaternion.identity;
        }
    }
}