using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, Interact
{
    [Header("Gate Settings")]
    public ItemData requiredKey;      // 보스가 떨구는 '그 열쇠' 데이터!
    public float dropDistance = 4f;   // 철창이 땅 밑으로 얼마나 꺼질지
    public float openSpeed = 2f;      // 드르륵 내려가는 속도

    [Header("Layer Settings")]
    public string interactLayerName = "Environment"; // 유니티에 설정해둔 상호작용 레이어 이름 (수정 필요!)

    public AudioSource gateAudio;
    public AudioClip gateMoveSound;

    private bool isOpen = false;
    private int interactLayer;
    private int defaultLayer;
    private PlayerController player;

    private void Start()
    {
        interactLayer = LayerMask.NameToLayer(interactLayerName);
        defaultLayer = LayerMask.NameToLayer("Default");

        player = FindObjectOfType<PlayerController>();

        // [띠또 마법 1] 시작할 때는 투명 인간(상호작용 불가) 취급합니다!
        gameObject.layer = defaultLayer;
    }

    private void Update()
    {
        if (isOpen || player == null) return;

        // [띠또 마법 2] 플레이어 손에 '그 열쇠'가 들려있을 때만 레이어를 바꿔서 E버튼이 뜨게 만듭니다!!
        if (player.currentWeapon == requiredKey)
        {
            gameObject.layer = interactLayer;
        }
        else
        {
            gameObject.layer = defaultLayer;
        }
    }

    public void Interact(PlayerController player)
    {
        if (isOpen) return;

        // 다시 한번 진짜 열쇠를 들고 있는지 확인!
        if (player.currentWeapon == requiredKey)
        {
            Debug.Log("철창이 열립니다!");
            ConsumeKey(player); // 열쇠 소모
            if (gateAudio != null && gateMoveSound != null)
            {
                gateAudio.PlayOneShot(gateMoveSound);
            }
            StartCoroutine(OpenGateRoutine()); // 문 열기 애니메이션
        }
    }

    private void ConsumeKey(PlayerController player)
    {
        int slotIdx = player.currentSlotIndex;
        if (slotIdx >= 0 && slotIdx < player.quickSlotCount.Length)
        {
            // 열쇠 개수 1개 깎기
            player.quickSlotCount[slotIdx]--;

            // 다 썼으면 슬롯 비우고 손에서도 없애기!
            if (player.quickSlotCount[slotIdx] <= 0)
            {
                player.quickSlot[slotIdx] = null;
                player.EquipItem(null);
            }

            // UI 업데이트
            if (player.quickSlotUI != null)
                player.quickSlotUI.UpdateQuickSlotUI(player.quickSlot, player.quickSlotCount);
        }
    }

    private IEnumerator OpenGateRoutine()
    {
        isOpen = true;
        gameObject.layer = defaultLayer; // 열리기 시작하면 E버튼 영구 삭제

        // 콜라이더를 바로 꺼버려서 플레이어가 기다리지 않고 쇽! 지나갈 수 있게 합니다
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos - new Vector3(0, dropDistance, 0);

        // 철창이 땅 밑으로 스르륵~ 내려가는 찰진 연출!!
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, openSpeed * Time.deltaTime);
            yield return null;
        }

        // 다 내려가면 흔적도 없이 소멸!!
        Destroy(gameObject);
    }
}