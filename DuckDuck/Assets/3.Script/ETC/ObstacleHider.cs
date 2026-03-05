using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))] // 카메라에 붙는 스크립트임을 명시
public class ObstacleHider : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public LayerMask hideLayer;
    [Range(0.1f, 5f)] public float castRadius = 2f;

    [Header("Magic Material")]
    public Material transparentMaterial;

    private Dictionary<MeshRenderer, Material[]> originalMaterials = new Dictionary<MeshRenderer, Material[]>();
    private List<MeshRenderer> currentHits = new List<MeshRenderer>();
    private Camera _cam;

    void Start()
    {
        _cam = GetComponent<Camera>();
    }

    void Update()
    {
        currentHits.Clear();

        // 1. [기존 방식] 카메라 -> 플레이어 위치 통나무 발사! (내 몸 시야 확보)
        Vector3 dirToPlayer = player.position - transform.position;
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        RaycastHit[] playerHits = Physics.SphereCastAll(transform.position, castRadius, dirToPlayer, distToPlayer, hideLayer);
        ProcessHits(playerHits);

        // 2. [주인님 아이디어 ⭐] 카메라 -> 마우스 조준점(크로스헤어) 통나무 발사! (에임 시야 확보)
        Ray mouseRay = _cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, player.position); // 플레이어 발밑 기준 바닥

        if (groundPlane.Raycast(mouseRay, out float distance))
        {
            Vector3 mouseTargetPos = mouseRay.GetPoint(distance);
            Vector3 dirToMouse = mouseTargetPos - transform.position;
            float distToMouse = Vector3.Distance(transform.position, mouseTargetPos);

            RaycastHit[] mouseHits = Physics.SphereCastAll(transform.position, castRadius, dirToMouse, distToMouse, hideLayer);
            ProcessHits(mouseHits);
        }

        // 3. 레이저에서 벗어난 건물들 원상 복구하기
        List<MeshRenderer> toRestore = new List<MeshRenderer>();
        foreach (var kvp in originalMaterials)
        {
            if (!currentHits.Contains(kvp.Key))
            {
                if (kvp.Key != null) kvp.Key.materials = kvp.Value; // 원래 옷 입혀주기
                toRestore.Add(kvp.Key);
            }
        }

        foreach (var mr in toRestore)
        {
            originalMaterials.Remove(mr);
        }
    }
    private void ProcessHits(RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            MeshRenderer[] renderers = hit.collider.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer mr in renderers)
            {
                if (mr != null)
                {
                    currentHits.Add(mr);

                    if (!originalMaterials.ContainsKey(mr))
                    {
                        originalMaterials.Add(mr, mr.materials); // 옷장에 넣고

                        Material[] transMats = new Material[mr.materials.Length];
                        for (int i = 0; i < transMats.Length; i++) transMats[i] = transparentMaterial;

                        mr.materials = transMats; // 투명 옷 입히기!
                    }
                }
            }
        }
    }
}