using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Map
{
    public class ChangeFieldColor : MonoBehaviour
    {
        // 该组件挂载在人物下，当人物达到特定条件时更改地块颜色

        //Map, Camp, Panel需要手动拖拽
        public GameObject _showMap;
        public GameObject _showCamp;
        public GameObject Panel;
        public Vector2 position;

        private DirectGenMap directGenMap;
        private int textureScale = 4;

        public int camp = 1;
        public int locationCamp = -1;

        private float maxTime = 0.5f;

        void Start()
        {
            directGenMap = Panel.GetComponent<DirectGenMap>();
            position = new Vector2(0, 0);
            textureScale = directGenMap.getTextureScale();
        }

        // 如果该物体在统一地块上停留超过一定时间，改变地块颜色
        private float timer = 0.2f;
        private Center bufferCenter;

        void Update()
        {
            // 检查directGenMap是否为空
            if (directGenMap == null)
            {
                Debug.Log("directGenMap is null");
                return;
            }

            // 找到最近的地块
            position = new Vector2((transform.position.x+75)*4, (transform.position.y+50)*4);
            bufferCenter = directGenMap.globalMap.Graph.ChangeCenterCamp(position, camp);


            // 维护timer，timer大于maxTime时改变颜色
            if (Time.time - timer >= maxTime)
            {
                // 异步加载改变颜色
                Debug.Log("Change color");
                timer = Time.time;

                Texture2D texture = _showCamp.GetComponent<Renderer>().material.mainTexture as Texture2D;
                StartCoroutine(ChangeColorAsync(bufferCenter, texture, camp));
                directGenMap.globalMap.Graph.DisplayCamp();
            }
        }

        // 根据所在Center，将该地块颜色改变为camp
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
                // 限制速度避免过大的占用
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