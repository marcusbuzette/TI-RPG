using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PathArrowMesh : MonoBehaviour {
    public float width = 0.4f;
    public float yOffset = 0.05f;
    public float arrowHeadLength = 0.5f; // tamanho da ponta da seta
    public float arrowHeadWidth = 0.8f;  // largura da base da seta

    public int curvesResolution = 4;

    public void DrawPath(List<GridPosition> path) {

        if (path == null || path.Count < 2) {
            return;
        }

        List<Vector3> pathPoints = new List<Vector3>();
        foreach (GridPosition position in path) {
            Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(position);
            worldPos.y += yOffset;
            pathPoints.Add(worldPos);
        }

        pathPoints = GetSmoothPath(pathPoints, resolution: 4);

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int vertIndex = 0;

        // Faixa do caminho
        for (int i = 0; i < pathPoints.Count - 1; i++) {
            Vector3 a = pathPoints[i];
            Vector3 b = pathPoints[i + 1];

            // Se for o último segmento, encurtar b para parar na base da seta
            if (i == pathPoints.Count - 2) {
                Vector3 direction = (b - a).normalized;
                b = b - direction * arrowHeadLength; // encurta o último segmento
            }

            Vector3 directionSegment = (b - a).normalized;
            Vector3 side = Vector3.Cross(directionSegment, Vector3.up) * width * 0.5f;

            Vector3 v1 = a - side;
            Vector3 v2 = a + side;
            Vector3 v3 = b - side;
            Vector3 v4 = b + side;

            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            triangles.Add(vertIndex + 0);
            triangles.Add(vertIndex + 1);
            triangles.Add(vertIndex + 2);

            triangles.Add(vertIndex + 1);
            triangles.Add(vertIndex + 3);
            triangles.Add(vertIndex + 2);

            vertIndex += 4;
        }

        // Ponta de seta ajustada: o vértice final é o último ponto do caminho
        Vector3 tipPoint = pathPoints[^1]; // Último ponto do caminho
        Vector3 from = pathPoints[^2];     // Penúltimo ponto
        Vector3 dir = (tipPoint - from).normalized;
        Vector3 normal = Vector3.up;
        Vector3 sideArrow = Vector3.Cross(dir, normal) * arrowHeadWidth * 0.5f;

        // A base da seta fica "atrás" do último ponto
        Vector3 baseCenter = tipPoint - dir * arrowHeadLength;
        Vector3 baseLeft = baseCenter - sideArrow;
        Vector3 baseRight = baseCenter + sideArrow;

        // Adiciona vértices da seta
        vertices.Add(baseLeft);
        vertices.Add(baseRight);
        vertices.Add(tipPoint);

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0.5f, 1));

        triangles.Add(vertIndex + 0);
        triangles.Add(vertIndex + 1);
        triangles.Add(vertIndex + 2);
        vertIndex += 3;

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    List<Vector3> GetSmoothPath(List<Vector3> points, int resolution = 5) {
        List<Vector3> smoothPath = new List<Vector3>();

        for (int i = 0; i < points.Count - 1; i++) {
            Vector3 p0 = i == 0 ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i + 2 < points.Count ? points[i + 2] : p2;

            for (int j = 0; j < resolution; j++) {
                float t = j / (float)resolution;
                Vector3 position = CatmullRom(p0, p1, p2, p3, t);
                smoothPath.Add(position);
            }
        }

        // Adiciona o último ponto para garantir que termina corretamente
        smoothPath.Add(points[points.Count - 1]);

        return smoothPath;
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }
}