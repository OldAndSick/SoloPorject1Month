using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PetGun : MonoBehaviour
{
    public Transform mouthpos;
    public LineRenderer laserLine;
    public GameObject hitEffectPrefab;
    public LayerMask enemyLayer;
    public Camera camShake;

    [Header("머신건 설정")]
    public float detectionRange = 20f;
    public float fireRate = 0.1f;
    public float bulletSpeed = 60f;
    public float spread = 0.05f;

    [Header("스킬 설정")]
    public float skiiDuration = 2f;
    public float cooldownTime = 30f;

    private float nextFireTime;
    private Transform target;
    private bool isFiring = false;
    private bool iscooldown = false;

    private void Update()
    {
        //q를 눌렀을때 발사 중이 아니고 쿨타임도 아니면 스킬 실행
        if(Input.GetKeyDown(KeyCode.Q)&&!isFiring&&!iscooldown)
        {
            StartCoroutine(SkillRoutine());
        }
        //스킬이 활성화된 상태에서만 적을 찾고 공격함
        if(isFiring)
        {
            FindNearestEnemy();
            if(target!=null)
            {
                if(Time.time>=nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

    IEnumerator SkillRoutine()
    {
        Debug.Log("헤비머신건 ㅌㅌㅌㅌ");
        isFiring = true;

        yield return new WaitForSeconds(skiiDuration);

        isFiring = false;
        iscooldown = true;
        Debug.Log("발열발열쿨타임쿨타임");

        //30초 쿨타임 대기
        yield return new WaitForSeconds(cooldownTime);
        iscooldown = false;
        Debug.Log("다시 사용 가능함");
    }

    void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        float shortestDistance = Mathf.Infinity;
        Transform nearesEnemy = null;

        foreach(Collider enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance<shortestDistance)
            {
                shortestDistance = distance;
                nearesEnemy = enemy.transform;
            }
        }
        target = nearesEnemy;
    }

    void Shoot()
    {
        if (target == null) return;

        Vector3 deviation = new Vector3
            (
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
            );
        Vector3 direction = ((target.position - mouthpos.position).normalized + deviation).normalized;

        RaycastHit hit;
        if(Physics.Raycast(mouthpos.position,direction,out hit,detectionRange,enemyLayer))
        {
            StartCoroutine(SpawnTracer(hit.point, hit.collider.gameObject));
        }

        //카메라 쉐이크 넣으려면... 카메라 스크립트에 쉐이크 메서드 있다고 가정할시
        //if (camShake != null) camShake.Shake(0.05f, 0.05f);
    }

    IEnumerator SpawnTracer(Vector3 hitPonint,GameObject enemyOBJ)
    {
        GameObject tracer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tracer.GetComponent<Collider>().enabled = false;
        tracer.transform.localScale = new Vector3(0.1f, 0.1f, 0.3f);

        Vector3 startPos = mouthpos.position;
        float distance = Vector3.Distance(startPos, hitPonint);
        float travelTime = distance / bulletSpeed;
        float elapsed = 0;

        while(elapsed<travelTime)
        {
            if (tracer == null) yield break;
            tracer.transform.position = Vector3.Lerp(startPos, hitPonint, elapsed / travelTime);
            tracer.transform.LookAt(hitPonint);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (hitEffectPrefab) Instantiate(hitEffectPrefab, hitPonint, Quaternion.identity);
        Destroy(tracer);
    }
}
