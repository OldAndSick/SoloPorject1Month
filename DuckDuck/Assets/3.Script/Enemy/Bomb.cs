using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float fuseTime = 2.0f;       //
    public float explosionRadius = 5f;  //
    public float damage = 40f;          //

    // [띠또 마법] 넌 누구 편이냐?! 
    // 체크(True)면 플레이어가 쏜 것, 체크 해제(False)면 부머가 쏜 것!
    [Header("팀킬 방지")]
    public bool isPlayerBomb = true;

    [Header("Effects")]
    public GameObject explosionEffectPrefab; //

    private void Start()
    {
        Invoke("Explode", fuseTime);
    }

    private void Explode()
    {
        if (explosionEffectPrefab != null) Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // [띠또 마법] 이미 맞은 녀석들을 기억하는 명단입니다!
        List<GameObject> hitObjects = new List<GameObject>();

        foreach (Collider hit in hitColliders)
        {
            // 1. 이미 명단에 있는 녀석(오브젝트)이면 무시하고 넘어간다!
            if (hitObjects.Contains(hit.gameObject)) continue;

            // 2. 처음 맞는 녀석이면 명단에 추가!
            hitObjects.Add(hit.gameObject);

            // --- 여기서부터는 기존 데미지 로직 ---
            if (isPlayerBomb)
            {
                if (hit.CompareTag("Enemy"))
                {
                    if (hit.TryGetComponent(out EnemyBase eb)) eb.TakeDamage(damage);
                }
                else if (hit.TryGetComponent(out Box box))
                {
                    box.TakeDamage(damage);
                }
            }
            else // 부머가 던진 폭탄일 때
            {
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
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}