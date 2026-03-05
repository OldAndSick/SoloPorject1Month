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

        if (Physics.Raycast(ray, out RaycastHit hit, moveDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            ProcessHit(hit.collider);
        }
        else
        {
            // 부딪힌 게 없으면 시원하게 전진!
            transform.Translate(Vector3.forward * moveDistance);
        }
    }

    // [핵심] Raycast와 OnTriggerEnter가 동시에 부르는 통합 충돌 처리 함수!
    private void ProcessHit(Collider other)
    {
        if (isHit) return;

        // 1. 진짜 정체 파악 (부모까지 뒤지기)
        PlayerController pc = other.GetComponentInParent<PlayerController>();
        EnemyBase eb = other.GetComponentInParent<EnemyBase>();
        EnemyAI eai = other.GetComponentInParent<EnemyAI>();

        // 2. 피아식별 (내 팀이면 무시하고 통과)
        if (!isEnemyBullet && pc != null) return;
        if (isEnemyBullet && (eb != null || eai != null)) return;

        if (other.isTrigger && pc == null && eb == null && eai == null)
        {
            return;
        }
        isHit = true;

        if (pc != null) pc.TakeDamage(damage);
        else if (eb != null) eb.TakeDamage(damage);
        else if (eai != null) eai.TakeDamage(damage);
        else if (other.TryGetComponent(out Box box)) box.TakeDamage(damage);
        else if (other.TryGetComponent(out DestroyObstacle obj)) obj.TakeDamage(damage);

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