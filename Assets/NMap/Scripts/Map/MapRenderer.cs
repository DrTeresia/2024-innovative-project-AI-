using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Map
{
    public class MapRenderer
    {
        public int textureScale = 10; // ��ԭ�����е�_textureScale��Ӧ
        public Material baseMaterial; // �������ʣ���������Shader

        // ����һ��ʹ��Sprite������ε���Ϸ����
        public GameObject CreateMapWithSprites(Map map)
        {
            GameObject mapContainer = new GameObject("MapContainer");

            // �����������Σ�ԭ�����еĵ�ɫ���֣�
            CreateBaseTerrain(map, mapContainer);

            // ��������α߽磨ԭ�����е�FillPolygon���֣�
            CreatePolygonBorders(map, mapContainer);

            return mapContainer;
        }

        private void CreateBaseTerrain(Map map, GameObject parent)
        {
            foreach (Center p in map.Graph.centers)
            {
                // Ϊÿ���������򴴽�һ�������
                GameObject regionObj = new GameObject($"Region_{p.index}");
                regionObj.transform.SetParent(parent.transform);

                // ���Mesh���
                MeshFilter meshFilter = regionObj.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = regionObj.AddComponent<MeshRenderer>();

                // ��ȡ��Ӧ����Ⱥϵ�Ĳ���
                Material biomeMaterial = GetBiomeMaterial(p.biome);
                meshRenderer.material = biomeMaterial;

                // �������������
                Mesh regionMesh = CreateRegionMesh(p);
                meshFilter.mesh = regionMesh;
            }
        }

        private void CreatePolygonBorders(Map map, GameObject parent)
        {
            foreach (Center p in map.Graph.centers)
            {
                foreach (var neighbor in p.neighbors)
                {
                    Edge edge = map.Graph.lookupEdgeFromCenter(p, neighbor);
                    if (edge == null || edge.v0 == null || edge.v1 == null)
                        continue;

                    // �����߽�����
                    Vector2[] polygonPoints = new Vector2[]
                    {
                        edge.v0.point,
                        edge.v1.point,
                        p.point
                    };

                    CreateBorderPolygon(polygonPoints, p.biome, parent.transform);
                }
            }
        }

        private Mesh CreateRegionMesh(Center center)
        {
            Mesh mesh = new Mesh();

            // ������εĽǵ�ת��Ϊ����
            List<Vector2> corners = center.corners.Select(c => c.point).ToList();
            Vector3[] vertices = corners.Select(v => new Vector3(v.x, v.y, 0)).ToArray();

            // �����ʷ�
            Triangulator triangulator = new Triangulator(corners.ToArray());
            int[] triangles = triangulator.Triangulate();

            // ����UV��ȷ��������ȷӳ�䣩
            Vector2[] uv = new Vector2[vertices.Length];
            Bounds bounds = new Bounds(vertices[0], Vector3.zero);
            foreach (Vector3 vertex in vertices)
            {
                bounds.Encapsulate(vertex);
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                uv[i] = new Vector2(
                    (vertices[i].x - bounds.min.x) / bounds.size.x,
                    (vertices[i].y - bounds.min.y) / bounds.size.y
                );
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void CreateBorderPolygon(Vector2[] points, Biome biome, Transform parent)
        {
            GameObject borderObj = new GameObject("BorderPolygon");
            borderObj.transform.SetParent(parent);

            // ���Mesh���
            MeshFilter meshFilter = borderObj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = borderObj.AddComponent<MeshRenderer>();

            // ʹ������Ⱥϵ�Ĳ���
            Material borderMaterial = GetBiomeMaterial(biome);
            meshRenderer.material = borderMaterial;

            // ��������
            Mesh borderMesh = new Mesh();
            Vector3[] vertices = points.Select(p => new Vector3(p.x, p.y, -0.1f)).ToArray(); // ��΢����Zֵȷ����ʾ���ϲ�

            // �������Σ�������3����Ķ���Σ�
            int[] triangles = new int[] { 0, 1, 2 };

            // UVӳ��
            Vector2[] uv = new Vector2[vertices.Length];
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2(
                    (vertices[i].x - vertices.Min(v => v.x)) / (vertices.Max(v => v.x) - vertices.Min(v => v.x)),
                    (vertices[i].y - vertices.Min(v => v.y)) / (vertices.Max(v => v.y) - vertices.Min(v => v.y))
                );
            }

            borderMesh.vertices = vertices;
            borderMesh.triangles = triangles;
            borderMesh.uv = uv;
            borderMesh.RecalculateNormals();

            meshFilter.mesh = borderMesh;
        }

        private Material GetBiomeMaterial(Biome biome)
        {
            // ���������������ȡ��Ӧ����Ⱥϵ�Ĳ���
            // �����Ԥ�ȴ�������������Ⱥϵ�Ĳ��ʣ����߶�̬����

            // ʾ��������һ���²��ʲ�������ɫ
            Material mat = new Material(baseMaterial);
            mat.color = BiomeProperties.Colors[biome];

            //// ������ж�Ӧ��Sprite����
            //Texture2D biomeTexture = BiomeProperties.earthTextures[biome].texture;
            //if (biomeTexture != null)
            //{
            //    mat.mainTexture = biomeTexture;
            //}

            return mat;
        }
    }

    // �����ʷ���
    public class Triangulator
    {
        private Vector2[] m_points;

        public Triangulator(Vector2[] points)
        {
            m_points = points;
        }

        public int[] Triangulate()
        {
            List<int> indices = new List<int>();

            int n = m_points.Length;
            if (n < 3) return indices.ToArray();

            int[] V = new int[n];
            if (Area() > 0)
            {
                for (int v = 0; v < n; v++)
                    V[v] = v;
            }
            else
            {
                for (int v = 0; v < n; v++)
                    V[v] = (n - 1) - v;
            }

            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2;)
            {
                if ((count--) <= 0)
                    return indices.ToArray();

                int u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                int w = v + 1;
                if (nv <= w)
                    w = 0;

                if (Snip(u, v, w, nv, V))
                {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);

                    for (s = v, t = v + 1; t < nv; s++, t++)
                        V[s] = V[t];
                    nv--;
                    count = 2 * nv;
                }
            }

            return indices.ToArray();
        }

        private float Area()
        {
            int n = m_points.Length;
            float A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                Vector2 pval = m_points[p];
                Vector2 qval = m_points[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }
            return (A * 0.5f);
        }

        private bool Snip(int u, int v, int w, int n, int[] V)
        {
            Vector2 A = m_points[V[u]];
            Vector2 B = m_points[V[v]];
            Vector2 C = m_points[V[w]];

            if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
                return false;

            for (int p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                    continue;
                Vector2 P = m_points[V[p]];
                if (InsideTriangle(A, B, C, P))
                    return false;
            }
            return true;
        }

        private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = C.x - B.x; ay = C.y - B.y;
            bx = A.x - C.x; by = A.y - C.y;
            cx = B.x - A.x; cy = B.y - A.y;
            apx = P.x - A.x; apy = P.y - A.y;
            bpx = P.x - B.x; bpy = P.y - B.y;
            cpx = P.x - C.x; cpy = P.y - C.y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }
    }


}
