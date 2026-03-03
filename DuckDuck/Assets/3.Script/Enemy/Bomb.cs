using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float fuseTime = 2.0f;       // 던져지고 몇 초 뒤에 터질지
    public float explosionRadius = 5f;  // 폭발 반경 (광역 딜)
    public float damage = 40f;          // 폭발 데미지

    [Header("Effects")]
    public GameObject explosionEffectPrefab; // 폭발 파티클(VFX)

    private void Start()
    {
        Invoke("Explode", fuseTime);
    }

    private void Explode()
    {
        // 1. 폭발 이펙트 펑!
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. 폭발 반경 안에 있는 모든 녀석들(콜라이더)을 찾습니다!
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            // 플레이어 타격!
            if (hit.CompareTag("Player"))
            {
                PlayerController pc = hit.GetComponent<PlayerController>();
                if (pc != null) pc.TakeDamage(damage);
            }
            else if (hit.TryGetComponent(out Box box))
            {
                box.TakeDamage(damage);
            }
        }

        // 3. 폭탄 자신은 장렬하게 산화!
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}