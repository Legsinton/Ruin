using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfViewVisualizer : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public int meshResolution = 50;
    public LayerMask obstructionMask;
    public float edgeFadeAmount = 0.3f; //(0 = sharp edge)

    private Mesh viewMesh;

    void Start()
    {
        viewMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = viewMesh;
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution / 360f);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        viewPoints.Add(Vector3.zero); // Center point (origin)

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(transform.InverseTransformPoint(newViewCast.point));
        }

        viewMesh.Clear();

        Vector3[] vertices = viewPoints.ToArray();
        int[] triangles = new int[(viewPoints.Count - 2) * 3];
        Color[] colors = new Color[vertices.Length];

        // Setup triangles
        for (int i = 0; i < viewPoints.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Setup vertex colors
        colors[0] = new Color(1, 1, 1, 1); // center = fully opaque
        for (int i = 1; i < colors.Length; i++)
        {
            float distFromCenter = Vector3.Distance(vertices[0], vertices[i]);
            float fade = Mathf.Clamp01(1 - (distFromCenter / viewRadius) / edgeFadeAmount);
            colors[i] = new Color(1, 1, 1, fade);
        }

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.colors = colors;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstructionMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool global)
    {
        if (!global)
            angleInDegrees += transform.eulerAngles.y;

        float rad = angleInDegrees * Mathf.Deg2Rad;
        return transform.right * Mathf.Sin(rad) + transform.forward * Mathf.Cos(rad);
    }


    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }
}