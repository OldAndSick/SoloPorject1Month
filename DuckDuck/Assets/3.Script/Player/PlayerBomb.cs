using UnityEngine;

public class PlayerBomb : MonoBehaviour
{
    [Header("폭탄 셋팅")]
    public GameObject bombPrefab; // 던질 폭탄 프리팹
    public Transform throwPoint;  // 던지는 위치 (손이나 카메라 앞)
    public float throwForce = 15f;

    [Header("포물선 & 범위 UI")]
    public LineRenderer trajectoryLine; // 궤적 그릴 선
    public Transform blastIndicator;    // 바닥에 생기는 원형 범위 UI
    public int linePoints = 30;

    public bool isAiming = false;

    void Update()
    {
        PlayerController pc = GetComponent<PlayerController>();

        // [철통 방어 1] 무기가 없거나, 이름이 폭탄이 아니거나, 슬롯 번호가 이상하면 절대 궤적 안 띄움!!
        if (pc == null || pc.currentWeapon == null || pc.currentWeapon.itemName != "Boomb")
        {
            CancelThrow();
            return;
        }

        // [철통 방어 2] 폭탄 개수가 0개 이하면 무조건 차단!!! (없는데 던지는 버그 완벽 해결)
        if (pc.quickSlotCount[pc.currentSlotIndex] <= 0)
        {
            CancelThrow();
            return;
        }
        // 1. 좌클릭 꾹 누르면 조준 시작
        if (Input.GetMouseButtonDown(0))
        {
            isAiming = true;
            trajectoryLine.enabled = true;
            blastIndicator.gameObject.SetActive(true);
        }

        // 2. 조준 중일 때
        if (isAiming)
        {
            DrawTrajectory();

            // 3. 좌클릭 떼면 투척!!
            if (Input.GetMouseButtonUp(0))
            {
                ThrowBomb();
            }

            // 4. [요청하신 기능] 숫자키 1, 2 누르면 조준 취소!!
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                CancelThrow();
            }
        }
    }

    void DrawTrajectory()
    {
        trajectoryLine.positionCount = linePoints;
        Vector3 startPos = throwPoint.position;
        Vector3 startVelocity = throwPoint.forward * throwForce;

        for (int i = 0; i < linePoints; i++)
        {
            float time = i * 0.1f; // 곡선 촘촘도
            Vector3 point = startPos + startVelocity * time + Physics.gravity * 0.5f * time * time;
            trajectoryLine.SetPosition(i, point);

            // 땅에 닿는 곳 예측해서 폭발 범위 UI 옮기기
            if (i > 0)
            {
                Vector3 prevPoint = trajectoryLine.GetPosition(i - 1);
                if (Physics.Raycast(prevPoint, point - prevPoint, out RaycastHit hit, Vector3.Distance(prevPoint, point)))
                {
                    trajectoryLine.positionCount = i + 1;
                    trajectoryLine.SetPosition(i, hit.point); // 선 끊기
                    blastIndicator.position = hit.point + Vector3.up * 0.1f; // 바닥에 범위 표시
                    blastIndicator.up = hit.normal; // 바닥 기울기에 맞추기
                    break;
                }
            }
        }
    }

    void ThrowBomb()
    {
        if (!isAiming) return;
        CancelThrow(); // 조준 끄기

        // 폭탄 생성 & 날리기 (원래 있던 코드)
        GameObject bomb = Instantiate(bombPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = throwPoint.forward * throwForce;
        }

        // [에러 방지] 퀵슬롯 서랍 번호가 정상일 때만 개수를 뺍니다!! (Index 에러 해결)
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null && pc.currentSlotIndex >= 0)
        {
            pc.quickSlotCount[pc.currentSlotIndex]--;

            if (pc.quickSlotCount[pc.currentSlotIndex] <= 0)
            {
                pc.quickSlotCount[pc.currentSlotIndex] = 0;
                pc.quickSlot[pc.currentSlotIndex] = null; // [띠또 마법] 데이터 확실히 비우기!
                pc.EquipItem(null); // 장착 해제
            }

            // [띠또 마법] 퀵슬롯 UI야, 지금 당장 새로 그려라!!
            if (pc.quickSlotUI != null) pc.quickSlotUI.UpdateQuickSlotUI(pc.quickSlot, pc.quickSlotCount);
        }
    }

    void CancelThrow()
    {
        isAiming = false;
        trajectoryLine.enabled = false;
        blastIndicator.gameObject.SetActive(false);
    }
}