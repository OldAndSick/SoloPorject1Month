using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))] // bo hum -- all jakk ddak
public class PlayerController : MonoBehaviour
{
    #region Header

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

    [Header("Inventory Setting")]
    public List<ItemData> inventory = new List<ItemData>();
    public GameObject inventoryUI;
    public GameObject slotPrefab;
    public Transform slotParent;

    [Header("Consumable Settings")]
    public int[] quickSlotCount = new int[9]; 
    public Slider castingBarUI; 
    private Coroutine castingCoroutine;

    [Header("UI Settings")]
    public RectTransform crosshairUI;
    public GameObject interactUI;

    [Header("QuickSlot Settings")]
    public ItemData[] quickSlot = new ItemData[9];
    public int currentSlotIndex = -1;
    public QuickSlotUI quickSlotUI;

    [Header("Interact Settings")]
    public float interactRange = 2f;
    public LayerMask interactLayer;

    [Header("Audio Settings")]
    public AudioSource playerAudio; // 플레이어 몸에 달린 스피커
    public AudioClip shootSound;    // 총 쏠 때 나는 소리
    public AudioClip rollSound;     // 구를 때 나는 소리
    public AudioClip walkSound;     // 걸을 때 나는 발소리
    public AudioClip reloadSound;

    private bool isInventoryOpen = false;
    private float lastAtackTime;
    private float regenTimer;
    private Rigidbody _rb;
    private Animator _ani;
    private Camera _mainCamera;
    private Vector3 _moveInput;
    private bool _isRolling = false;
    private float stepTimer = 0f;
    public float stepInterval = 0.4f;

    private static readonly int ANIM_SPEED = Animator.StringToHash("Speed");
    private static readonly int ANIM_ROLL = Animator.StringToHash("Roll");
    #endregion

    private void Start()
    {
        currentStamina = maxStamina;
        currentHP = maxHP;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        if (inventoryUI != null) inventoryUI.SetActive(false);

        if (MoveData.hasData)
        {
            currentHP = MoveData.savedHP;

            quickSlot = (ItemData[])MoveData.savedQuickSlot.Clone();
            quickSlotCount = (int[])MoveData.savedQuickSlotCount.Clone();

            currentWeapon = MoveData.savedWeapon;
            currentMag = MoveData.savedCurrentMag;
            totalAmmo = MoveData.savedTotalAmmo;
            currentSlotIndex = MoveData.savedSlotIndex;

            if (currentWeapon != null)
            {
                EquipItem(currentWeapon);
            }
            if (quickSlotUI != null)
            {
                quickSlotUI.UpdateQuickSlotUI(quickSlot, quickSlotCount);
                if (currentSlotIndex != -1) quickSlotUI.HighlightSlot(currentSlotIndex);
            }
            UpdateInventoryUI(); 
        }
        else
        {
            currentStamina = maxStamina;
            currentHP = maxHP;
            currentMag = 0;
            totalAmmo = 0;
        }
        UpdateHPUI();
        UpdateAmmoUI();
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
        if (crosshairUI != null)
        {
            crosshairUI.position = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        if (isInventoryOpen || _isRolling) return;
        HandleQuickSlotInput();
        HandleRotation();
        HandleInput();
        HandleCombat();
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadRoutine());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
        CheckInteractableUI();
    }

    private void FixedUpdate()
    {
        if (_isRolling) return;
        MovePlayer();
        HandleStamina();
    }

    #region PlayerMove
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
        if (_moveInput.sqrMagnitude > 0 && !_isRolling)
        {
            stepTimer -= Time.fixedDeltaTime;
            if (stepTimer <= 0f)
            {
                if (playerAudio != null && walkSound != null)
                {
                    // 뛸 때는 소리를 살짝 더 크게(1.0f), 걸을 때는 작게(0.6f)
                    float volume = isSprinting ? 1.0f : 0.6f;
                    playerAudio.PlayOneShot(walkSound, volume);
                }
                // 뛰면 발소리가 더 빨리 나게 타이머 조절
                stepTimer = isSprinting ? stepInterval * 0.7f : stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // 멈추면 타이머 초기화
        }
    }

    private IEnumerator RollRoutine()
    {
        _isRolling = true;
        if (playerAudio != null && rollSound != null)
        {
            playerAudio.PlayOneShot(rollSound);
        }
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
        else if (regenTimer > 0)
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
    #endregion
    #region Battle
    private void PerformMeleeAttack()
    {
        if (Time.time < lastAtackTime + attackCooldown) return;

        lastAtackTime = Time.time;
        if (slashVFXPrefab != null && attackPoint != null)
        {
            Instantiate(slashVFXPrefab, attackPoint.position, attackPoint.rotation);
        }
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        Debug.Log($"공격 범위 내 감지된 콜라이더 수: {hitEnemies.Length}");
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemyAI enemyAI))
            {
                enemyAI.TakeDamage(attackDamage);
                Debug.Log($"{enemy.name}(일반)에게 {attackDamage} 데미지!");
            }
            else if (enemy.TryGetComponent(out EnemyBase enemyTarget))
            {
                enemyTarget.TakeDamage(attackDamage);
            }
            else if (enemy.TryGetComponent(out Box box))
            {
                box.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"아야! 현재 체력: {currentHP}");
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHPUI();
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void HandleCombat()
    {
        if (Time.timeScale == 0f) return;
        PlayerBomb bombSys = GetComponent<PlayerBomb>();
        if (bombSys != null && bombSys.isAiming) return;
        if (isReloading) return;
        if (currentWeapon != null && currentWeapon.itemName == "Boomb") return;
        if (currentWeapon != null && currentWeapon.type == ItemData.ItemType.Gun)
        {
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentWeapon != null && currentWeapon.type == ItemData.ItemType.Consumable)
                {
                    if (castingCoroutine == null && quickSlotCount[currentSlotIndex] > 0)
                        castingCoroutine = StartCoroutine(UseItemRoutine(currentWeapon));
                }
                else
                {
                    PerformMeleeAttack();
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && castingCoroutine != null)
        {
            StopCoroutine(castingCoroutine);
            castingCoroutine = null;
            if (castingBarUI != null) castingBarUI.gameObject.SetActive(false);
        }
    }
    #endregion
    #region Gun

    private void Shoot()
    {
        if (currentMag <= 0)
        {
            Debug.Log("탄없");
            return;
        }

        float currentFireRate = (currentWeapon != null && currentWeapon.fireRate > 0) ? currentWeapon.fireRate : attackCooldown;
        if (Time.time < lastAtackTime + currentFireRate) return;
        if (currentWeapon != null && currentWeapon.muzzleFlashPrefab != null)
        {
            // attackPoint 위치에 화염을 생성합니다.
            GameObject flash = Instantiate(currentWeapon.muzzleFlashPrefab, attackPoint.position, attackPoint.rotation);

            // 화염이 플레이어를 따라다니게 하려면 부모를 설정해 줍니다.
            flash.transform.SetParent(attackPoint);

            // 0.1초 뒤에 화염 오브젝트를 자동으로 삭제합니다. (짧고 굵게!)
            Destroy(flash, 0.1f);
        }
        currentMag--;
        UpdateAmmoUI();

        if (playerBulletPrefab == null) Debug.LogError("playerBulletPrefab이 비어있다 이놈아!");
        if (attackPoint == null) Debug.LogError("attackPoint가 비어있다 이놈아!");

        if (playerBulletPrefab != null && attackPoint != null)
        {
            float currentSpread = (currentWeapon != null) ? currentWeapon.gunSpread : bulletSpread;

            float spreadX = Random.Range(-currentSpread, currentSpread);
            float spreadY = Random.Range(-currentSpread, currentSpread);
            Quaternion spreadRotation = Quaternion.Euler(spreadX, spreadY, 0);

            GameObject bullet = Instantiate(playerBulletPrefab, attackPoint.position, attackPoint.rotation * spreadRotation);
            if (playerAudio != null && shootSound != null)
            {
                playerAudio.PlayOneShot(shootSound);
            }
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null && currentWeapon != null)
            {
                bulletScript.damage = currentWeapon.gunDamage;
                bulletScript.speed = currentWeapon.gunSpeed;
            }

            Debug.Log("shoot");
        }

        lastAtackTime = Time.time;
    }
    IEnumerator ReloadRoutine()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("주인님! 맨손인데 장전을 시도했습니다!");
            yield break;
        }
        if (totalAmmo <= 0 || currentMag == currentWeapon.magSize) yield break;

        isReloading = true;
        if (playerAudio != null && reloadSound != null)
        {
            playerAudio.PlayOneShot(reloadSound);
        }
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
        if (ammoUI == null) return;
        bool isGun = currentWeapon != null && currentWeapon.type == ItemData.ItemType.Gun;
        ammoUI.gameObject.SetActive(isGun);
        if (isGun)
        {
            ammoUI.text = $"{currentMag} / {totalAmmo}";
        }
    }
    private void UpdateWeaponModel(GameObject prefab)
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
        if (prefab != null && weaponHolder != null)
        {
            currentWeaponModel = Instantiate(prefab, weaponHolder.transform);
            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation = Quaternion.identity;
        }
    }

    #endregion
    #region Inven

    public void AcquireItem(ItemData data)
    {
        if (data == null) return;

        // 1. 퀘스트 아이템: 인벤토리 리스트에 추가
        if (data.type == ItemData.ItemType.Quest)
        {
            inventory.Add(data);
            UpdateInventoryUI();
            return;
        }

        // 2. 소비 아이템(힐템): 퀵슬롯에 이미 있으면 개수만 올림
        if (data.type == ItemData.ItemType.Consumable)
        {
            for (int i = 0; i < quickSlot.Length; i++)
            {
                // 이름이 똑같은 힐템을 퀵슬롯에서 찾습니다.
                if (quickSlot[i] != null && quickSlot[i].itemName == data.itemName)
                {
                    quickSlotCount[i]++;
                    if (quickSlotUI != null) quickSlotUI.UpdateQuickSlotUI(quickSlot, quickSlotCount);
                    return;
                }
            }
        }

        // 3. 총기 아이템: 이미 있으면 해당 총의 탄약만 추가
        if (data.type == ItemData.ItemType.Gun)
        {
            for (int i = 0; i < quickSlot.Length; i++)
            {
                // 퀵슬롯에 이미 있는 총인지 확인 (예: AK를 이미 가졌는지)
                if (quickSlot[i] != null && quickSlot[i].itemName == data.itemName)
                {
                    // 그 총의 데이터에 탄약을 직접 더해줍니다!
                    quickSlot[i].currentTotalAmmo += data.startTotalAmmo;

                    // [중요] 만약 지금 그 총을 들고 있다면? 실시간 변수도 동기화!
                    if (currentWeapon == quickSlot[i])
                    {
                        totalAmmo = quickSlot[i].currentTotalAmmo;
                        UpdateAmmoUI();
                    }
                    return;
                }
            }
        }

        // 4. 아예 처음 먹는 아이템이면? 빈 슬롯에 새로 등록
        AddQuickSlot(data);

        // 방금 새로 먹은 게 총이라면 초기 탄약 세팅
        if (data.type == ItemData.ItemType.Gun)
        {
            data.currentMagCount = data.magSize;
            data.currentTotalAmmo = data.startTotalAmmo;

            if (currentWeapon == data)
            {
                currentMag = data.currentMagCount;
                totalAmmo = data.currentTotalAmmo;
                UpdateAmmoUI();
            }
        }
    }
    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(isInventoryOpen);
        }
        if (isInventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }

    }
    private void UpdateInventoryUI()
    {
        if (slotParent == null || slotPrefab == null) return;

        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        foreach (ItemData item in inventory)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            newSlot.GetComponent<InventorySlot>().SetItem(item);
        }
    }
    public void EquipItem(ItemData data)
    {
        if (currentWeapon != null && currentWeapon.type == ItemData.ItemType.Gun)
        {
            currentWeapon.currentMagCount = currentMag;
            currentWeapon.currentTotalAmmo = totalAmmo;
        }
        currentWeapon = data;
        if (data == null)
        {
            UpdateWeaponModel(null);
            UpdateAmmoUI();
            return;
        }
        UpdateWeaponModel(data.weaponPrefab);

        if (data.type == ItemData.ItemType.Gun)
        {
            currentMag = data.currentMagCount;
            totalAmmo = data.currentTotalAmmo;
            UpdateAmmoUI();
        }
    }
    private void HandleQuickSlotInput()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectQuickSlot(i);
            }
        }
    }
    public void SelectQuickSlot(int index)
    {
        if (index < 0 || index >= quickSlot.Length) return;

        currentSlotIndex = index;
        ItemData item = quickSlot[index];

        EquipItem(item);
        if (quickSlotUI != null) quickSlotUI.HighlightSlot(index);
        Debug.Log($"{(index + 1)}번 슬롯 선택: {(item != null ? item.itemName : "맨손")}");
    }
    public void AddQuickSlot(ItemData item)
    {
        if (item == null) return;
        Debug.Log($"AddQuickSlot 실행됨: {item.itemName}");
        for (int i = 0; i < quickSlot.Length; i++)
        {
            if (quickSlot[i] == item) return;
        }
        for (int i = 0; i < quickSlot.Length; i++)
        {
            if (quickSlot[i] == null)
            {
                quickSlot[i] = item;
                quickSlotCount[i] = 1;
                if (quickSlotUI != null) quickSlotUI.UpdateQuickSlotUI(quickSlot, quickSlotCount);
                return;

            }
        }
    }

    private IEnumerator UseItemRoutine(ItemData item)
    {
        float useTime = item.useTime > 0 ? item.useTime : 2.0f; // 기본 2초
        float timer = 0f;

        if (castingBarUI != null)
        {
            castingBarUI.gameObject.SetActive(true);
            castingBarUI.value = 0f;
        }

        while (timer < useTime)
        {
            // run roll -> cancel
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && _moveInput.sqrMagnitude > 0;

            if (_isRolling || isSprinting)
            {
                if (castingBarUI != null) castingBarUI.gameObject.SetActive(false);
                castingCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            if (castingBarUI != null) castingBarUI.value = timer / useTime;
            yield return null;
        }

        currentHP += item.healAmount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHPUI();
        //-- count
        quickSlotCount[currentSlotIndex]--;
        // use all -> make empty hand
        if (quickSlotCount[currentSlotIndex] <= 0)
        {
            quickSlot[currentSlotIndex] = null;
            EquipItem(null);
        }

        if (quickSlotUI != null) quickSlotUI.UpdateQuickSlotUI(quickSlot, quickSlotCount);

        if (castingBarUI != null) castingBarUI.gameObject.SetActive(false);
        castingCoroutine = null;
    }
    #endregion
    private void UpdateUI()
    {
        if (staminaRing == null || uiGroup == null)
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
    private void TryInteract()
    {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, interactRange, interactLayer);

        foreach (Collider hit in hitCollider)
        {
            Interact interactable = hit.GetComponent<Interact>();
            if (interactable != null)
            {
                interactable.Interact(this);
                break;
            }
        }
    }
    private void CheckInteractableUI()
    {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, interactRange, interactLayer);
        Collider closetInteract = null;
        float minDis = Mathf.Infinity;

        foreach (Collider hit in hitCollider) //find close target
        {
            if (hit.GetComponent<Interact>() != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < minDis)
                {
                    minDis = distance;
                    closetInteract = hit;
                }
            }
        }

        if (closetInteract != null) //ui move
        {
            if (!interactUI.activeSelf) interactUI.SetActive(true);
            Vector3 centerPos = closetInteract.bounds.center;
            Vector3 dirToCamera = (_mainCamera.transform.position - centerPos).normalized;
            interactUI.transform.position = centerPos + (Vector3.up * 0.5f) + (dirToCamera * 1f);
            interactUI.transform.rotation = _mainCamera.transform.rotation;
        }
        else
        {
            if (interactUI.activeSelf) interactUI.SetActive(false);
        }
    }

    public void SavePlayerDataToTransfer()
    {
        MoveData.savedHP = currentHP;

        MoveData.savedQuickSlot = (ItemData[])quickSlot.Clone();
        MoveData.savedQuickSlotCount = (int[])quickSlotCount.Clone();

        MoveData.savedWeapon = currentWeapon;
        MoveData.savedCurrentMag = currentMag;
        MoveData.savedTotalAmmo = totalAmmo;
        MoveData.savedSlotIndex = currentSlotIndex;

        MoveData.hasData = true;
    }
}