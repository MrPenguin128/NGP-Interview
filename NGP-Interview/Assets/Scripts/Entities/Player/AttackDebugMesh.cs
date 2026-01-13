using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AttackDebugMesh : Singleton<AttackDebugMesh>
{
    [SerializeField] private int segments = 20;
    private Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void DrawArc(float range, float angle)
    {
        mesh.Clear();

        int vertexCount = segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float halfAngle = angle * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (angle / segments) * i;
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
            vertices[i + 1] = dir * range;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Invoke(nameof(Clear), 0.1f);
    }

    public void Clear()
    {
        mesh.Clear();
    }
}
