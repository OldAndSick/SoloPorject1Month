using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public float damage = 10f;

    // [중요] 누가 쏜 총알인지 구분하기 위해! 
    // 저격수가 쏠 땐 true, 플레이어가 쏠 땐 false로 프리팹을 나누거나 세팅해주세요.
    public bool isEnemyBullet = true;

    private bool isHit = false; // 중복 충돌 방지

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (isHit) return;

        float moveDistance = speed * Time.deltaTime;
        Ray ray = new Ray(transform.position, transform.forward);

        // 1. Raycast로 초고속 명중 체크 (터널링 방지)
        if (Physics.Raycast(ray, out RaycastHit hit, moveDistance))
        {
            ProcessHit(hit.collider);
        }
        else
        {
            // 2. 아무것도 없으면 전진
            transform.Translate(Vector3.forward * moveDistance);
        }
    }

    // [핵심] Raycast와 OnTriggerEnter가 동시에 부르는 통합 충돌 처리 함수!
    private void ProcessHit(Collider other)
    {
        if (isHit) return;

        // 쏜 사람 본인(저격수면 Enemy, 플레이어면 Player)은 통과!
        if (isEnemyBullet && other.CompareTag("Enemy")) return;
        if (!isEnemyBullet && other.CompareTag("Player")) return;

        isHit = true; // 처리 시작했으니 락 걸기

        // A. 플레이어 피격 체크 (어느 부위든 부모인 PlayerController를 찾음)
        PlayerController pc = other.GetComponentInParent<PlayerController>();
        if (pc != null)
        {
            pc.TakeDamage(damage);
            DestroyBullet();
            return;
        }

        // B. 적군(Enemy) 피격 체크 (조상님 EnemyBase 포함)
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out EnemyAI eai)) eai.TakeDamage(damage);
            else if (other.TryGetComponent(out EnemyBase eb)) eb.TakeDamage(damage);
            DestroyBullet();
            return;
        }

        // C. 기타 파괴 가능 물체 (박스, 장애물 등)
        if (other.TryGetComponent(out Box box)) box.TakeDamage(damage);
        else if (other.TryGetComponent(out DestroyObstacle obj)) obj.TakeDamage(damage);

        // D. 바닥이나 벽(Default, Ground)에 맞으면 즉시 소멸
        DestroyBullet();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Raycast가 놓칠 수도 있는 느린 충돌은 여기서 잡습니다!
        ProcessHit(other);
    }

    private void DestroyBullet()
    {
        // 필요하다면 여기서 피격 이펙트(VFX)를 생성하세요!
        Destroy(gameObject);
    }
}