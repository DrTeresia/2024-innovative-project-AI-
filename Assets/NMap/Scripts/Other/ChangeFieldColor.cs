using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Map
{
    public class ChangeFieldColor : MonoBehaviour
    {
        // ����������������£�������ﵽ�ض�����ʱ���ĵؿ���ɫ

        //Map, Camp, Panel��Ҫ�ֶ���ק
        public GameObject _showMap;
        public GameObject _showCamp;
        public GameObject Panel;
        public Vector2 position;

        private DirectGenMap directGenMap;
        private int textureScale = 10;

        public int camp = 1;
        public int locationCamp = -1;
        public float locationElevation = 0.0f;
        public float locationMoisture = 0.0f;
        public Biome locationBiome = Biome.Ocean;

        void Start()
        {
            directGenMap = Panel.GetComponent<DirectGenMap>();
            position = new Vector2(0, 0);
            textureScale = directGenMap.getTextureScale();
        }

        // �����������ͳһ�ؿ���ͣ������һ��ʱ�䣬�ı�ؿ���ɫ
        private float timer = 0.0f;
        private float timerMax = 4.0f;
        private Center bufferCenter;
        private Center currentCenter;

        void Update()
        {
            // ���directGenMap�Ƿ�Ϊ��
            if (directGenMap == null)
            {
                Debug.Log("directGenMap is null");
                return;
            }

            // �ҵ�����ĵؿ�
            position = new Vector2((transform.position.x+75)*4, (transform.position.y+50)*4);

            // ����Ƿ��ڲ�ͬ�ؿ��ڶ���һ��ʱ��
            currentCenter = directGenMap.globalMap.Graph.ChangeCenterCamp(position, camp);

            if (currentCenter != null && bufferCenter == null)
            {
                bufferCenter = currentCenter;
            }
            if (currentCenter != null && bufferCenter != null && currentCenter != bufferCenter)
            {
                bufferCenter = currentCenter;
                timer = 0.0f;
            }
            if (currentCenter != null && bufferCenter != null && currentCenter == bufferCenter)
            {
                timer += Time.deltaTime;
                if (timer > timerMax)
                {
                    // ��ȡ��ǰ�ؿ�Ĳ��ʣ� ��_showCamp��ȡ
                    Texture2D texture = _showCamp.GetComponent<Renderer>().material.mainTexture as Texture2D;
                    StartCoroutine(ChangeColorAsync(currentCenter, texture, camp));
                    timer = 0.0f;
                }
            }
            locationCamp = currentCenter.camp;
            locationBiome = currentCenter.biome;
            locationElevation = currentCenter.elevation;
            locationMoisture = currentCenter.moisture;
        }

        // ��������Center�����õؿ���ɫ�ı�Ϊcamp
        private IEnumerator ChangeColorAsync(Center center, Texture2D texture, int camp)
        {
           if (center == null)
            {
                Debug.Log("Center is null");
                yield break;
            }
            List<Vector2> vertices = new List<Vector2>();
            Graph g = directGenMap.globalMap.Graph;
            foreach (var r in center.neighbors)
            {
                vertices.Clear();
                Edge edge = g.lookupEdgeFromCenter(center, r);
                if (edge == null)
                    continue;
                vertices.Add(edge.v0.point);
                vertices.Add(edge.v1.point);
                vertices.Add(center.point);
                texture.FillPolygon(vertices.Select(x => new Vector2(x.x * textureScale, x.y * textureScale)).ToArray(), Camp.Colors[camp]);
            }
            texture.Apply();
            yield return null;
        }

        private IEnumerator ChangePolygonColorAsync(Texture2D texture, Vector2[] vertices, Color color)
        {
            for (int x = 0; x < 600; x++)
            {
                for (int y = 0; y < 400; y++)
                {
                    if (IsPointInPolygon(new Vector2(x, y), vertices))
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
                // �����ٶȱ�������ռ��
                if (x % 10 == 0)
                    Debug.Log("Change color " + x);
                yield return null;
            }
            Debug.Log("Change color done");
            texture.Apply();
            yield return null;
        }

        bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
        {
            int polygonLength = polygon.Length, i = 0;
            bool inside = false;
            float pointX = point.x, pointY = point.y;
            float startX, startY, endX, endY;
            Vector2 endPoint = polygon[polygonLength - 1];
            endX = endPoint.x;
            endY = endPoint.y;
            while (i < polygonLength)
            {
                startX = endX; startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.x; endY = endPoint.y;
                inside ^= (endY > pointY ^ startY > pointY) && ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }
    }
}