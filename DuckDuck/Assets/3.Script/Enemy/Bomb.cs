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

        // [띠또 마법 ⭐] 게임오브젝트 대신 '스크립트 본체'를 명단에 적습니다! (중복 타격 완벽 방지)
        List<Component> hitTargets = new List<Component>();

        foreach (Collider hit in hitColliders)
        {
            if (isPlayerBomb) // 플레이어가 던진 폭탄일 때
            {
                // 1. 부모, 자식 상관없이 알맹이(스크립트)를 무조건 찾아냅니다!
                EnemyBase eb = hit.GetComponentInParent<EnemyBase>();
                EnemyAI eai = hit.GetComponentInParent<EnemyAI>(); // (혹시 EnemyAI 쓰는 적이 있을까봐 추가!)

                // 2. 적을 찾았다면?
                if (eb != null)
                {
                    if (hitTargets.Contains(eb)) continue; // 이미 때린 적이면 무시
                    hitTargets.Add(eb);
                    eb.TakeDamage(damage);
                }
                else if (eai != null)
                {
                    if (hitTargets.Contains(eai)) continue;
                    hitTargets.Add(eai);
                    eai.TakeDamage(damage);
                }
                else if (hit.TryGetComponent(out Box box))
                {
                    if (hitTargets.Contains(box)) continue;
                    hitTargets.Add(box);
                    box.TakeDamage(damage);
                }
            }
            else // 부머(적)가 던진 폭탄일 때
            {
                PlayerController pc = hit.GetComponentInParent<PlayerController>();
                if (pc != null)
                {
                    if (hitTargets.Contains(pc)) continue;
                    hitTargets.Add(pc);
                    pc.TakeDamage(damage);
                }
                else if (hit.TryGetComponent(out Box box))
                {
                    if (hitTargets.Contains(box)) continue;
                    hitTargets.Add(box);
                    box.TakeDamage(damage);
                }
            }
        }
        Destroy(gameObject); // 펑!
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}