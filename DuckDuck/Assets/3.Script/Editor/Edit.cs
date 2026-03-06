using UnityEngine;
using UnityEditor;

public class RemoveLODWizard : Editor
{
    // 유니티 상단 메뉴에 '띠또툴'이라는 마법의 버튼을 만듭니다!
    [MenuItem("띠또툴/선택한 오브젝트 LOD 싹다 지우기")]
    public static void RemoveLOD()
    {
        int count = 0;
        // 선택한 오브젝트와 그 자식들 속에 숨은 LOD Group을 싹 다 찾아냅니다!
        LODGroup[] lodGroups = Selection.GetFiltered<LODGroup>(SelectionMode.Deep);

        foreach (LODGroup lod in lodGroups)
        {
            LOD[] lods = lod.GetLODs();

            // [핵심] LOD1, LOD2 등 퀄리티 낮은 찌끄레기 메쉬들 사형! (가장 고퀄인 LOD0은 살려둠)
            for (int i = 1; i < lods.Length; i++)
            {
                foreach (Renderer r in lods[i].renderers)
                {
                    if (r != null) DestroyImmediate(r.gameObject);
                }
            }
            // 마지막으로 쓸모없어진 LOD Group 컴포넌트 목 치기!
            DestroyImmediate(lod);
            count++;
        }
        Debug.Log($"[띠또 보고서] 총 {count}개의 LOD를 완벽하게 숙청했습니다요!!");
    }
}