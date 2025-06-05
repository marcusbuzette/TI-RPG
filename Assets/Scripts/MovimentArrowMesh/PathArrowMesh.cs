using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PathArrowMesh : MonoBehaviour {
    public float width = 0.2f;

    public void DrawPath(List<GridPosition> path) {

        if (path == null || path.Count < 2) {
            Debug.LogWarning("Caminho muito curto para desenhar.");
            return;
        }

        List<Vector3> pathPoints = new List<Vector3>();

        foreach (GridPosition position in path) {
            pathPoints.Add(LevelGrid.Instance.GetWorldPosition(position));
        }

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int vertIndex = 0;

        for (int i = 0; i < pathPoints.Count - 1; i++) {
            Vector3 a = pathPoints[i];
            Vector3 b = pathPoints[i + 1];

            Vector3 direction = (b - a).normalized;
            Vector3 normal = Vector3.up;
            Vector3 side = Vector3.Cross(direction, normal) * width * 0.5f;

            // Criar 2 vértices para cada lado da faixa
            Vector3 v1 = a - side;
            Vector3 v2 = a + side;
            Vector3 v3 = b - side;
            Vector3 v4 = b + side;

            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);

            // UVs (simples, para textura)
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            // Dois triângulos por segmento
            triangles.Add(vertIndex + 0);
            triangles.Add(vertIndex + 1);
            triangles.Add(vertIndex + 2);

            triangles.Add(vertIndex + 1);
            triangles.Add(vertIndex + 3);
            triangles.Add(vertIndex + 2);


            vertIndex += 4;
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}