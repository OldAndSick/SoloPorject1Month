using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("FOV Set")]
    public float viewRadius = 10f;       
    public float nearViewRadius = 2f;    
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Mesh Set")]
    public float meshResolution = 1f;
    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private void DrawFieldOfView()
    {
        List<Vector3> allPoints = new List<Vector3>();

        int nearStepCount = 16;
        float nearStepAngle = 360f / nearStepCount;
        for (int i = 0; i <= nearStepCount; i++)
        {
            float angle = i * nearStepAngle;
            allPoints.Add(ViewCast(angle, nearViewRadius).point);
        }

        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            allPoints.Add(ViewCast(angle, viewRadius).point);
        }

        int vertexCount = allPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0, 0);
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(allPoints[i]);

            float dist = Vector3.Distance(Vector3.zero, vertices[i + 1]) / viewRadius;
            uvs[i + 1] = new Vector2(dist, 0);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.uv = uvs;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle, float radius)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(transform.position, dir, out hit, radius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point + Vector3.up * 0.1f, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, origin + dir * radius, radius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal) angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;
        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
}