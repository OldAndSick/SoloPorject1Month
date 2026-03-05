using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class PetGun : MonoBehaviour
{
    public Transform mouthpos;
    public LineRenderer laserLine;
    public GameObject hitEffectPrefab;
    public LayerMask enemyLayer;
    public Camera camShake;

    [Header("�ӽŰ� ����")]
    public float detectionRange = 20f;
    public float fireRate = 0.1f;
    public float bulletSpeed = 60f;
    public float spread = 0.05f;

    [Header("��ų ����")]
    public float skiiDuration = 2f;
    public float cooldownTime = 30f;

    private float nextFireTime;
    private Transform target;

    [Header("UI����")]
    public Image cooldownImage;

    private bool isFiring = false;
    private bool iscooldown = false;

    private void Update()
    {
        //q�� �������� �߻� ���� �ƴϰ� ��Ÿ�ӵ� �ƴϸ� ��ų ����
        if(Input.GetKeyDown(KeyCode.Q)&&!isFiring&&!iscooldown)
        {
            StartCoroutine(SkillRoutine());
        }
        //��ų�� Ȱ��ȭ�� ���¿����� ���� ã�� ������
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
        Debug.Log("���ӽŰ� ��������");
        isFiring = true;

        yield return new WaitForSeconds(skiiDuration);

        isFiring = false;
        iscooldown = true; // �빮�� ��Ÿ ����! (isCooldown)
        Debug.Log("�߿��߿���Ÿ����Ÿ��");

        // --- ������� �ǽð� UI ���� ���� ---
        float timer = cooldownTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime; // �� ������ �ð��� ����

            if (cooldownImage != null)
            {
                // (�����ð� / ��ü�ð�) ������ �̹��� ä���
                cooldownImage.fillAmount = timer / cooldownTime;
            }

            yield return null; // ���� �����ӱ��� ���
        }
        // --------------------------------

        if (cooldownImage != null) cooldownImage.fillAmount = 0; // Ȯ���� ����ֱ�
        iscooldown = false;
        Debug.Log("�ٽ� ��� ������");
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

        //ī�޶� ����ũ ��������... ī�޶� ��ũ��Ʈ�� ����ũ �޼��� �ִٰ� �����ҽ�
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
