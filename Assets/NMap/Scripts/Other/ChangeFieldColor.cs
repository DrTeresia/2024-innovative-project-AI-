using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Map{
    public class ChangeFieldColor : MonoBehaviour
    {
        // 该组件挂载在人物下，当人物达到特定条件时更改地块颜色
        private Vector2 position;
        private GameObject _showMap;
        private GameObject _showCamp;
        private GameObject Panel;
        private DirectGenMap directGenMap;

        public int camp = -1;
        public int locationCamp = -1;

        private float maxTime = 1.0f;

        void Start()
        {
            _showMap = GameObject.Find("Map");
            _showCamp = GameObject.Find("CampMap");
            Panel = GameObject.Find("Canvas/Panel");
            directGenMap = Panel.GetComponent<DirectGenMap>();
        }

        // 如果该物体在统一地块上停留超过一定时间，改变地块颜色
        private float timer = 0;
        private Center bufferCenter;
        void Update()
        {
            // 通过坐标找到对应的地块
            Center center = directGenMap.globalMap.Graph.FindCenter(position);
            if (center != bufferCenter)
            {
                bufferCenter = center;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= maxTime)
                {
                    ChangeColor(position, camp);
                }
            }
            //LocationCamp();
        }

        // 根据坐标改变地块颜色
        public void ChangeColor(Vector2 position, int camp)
        {
            // 通过坐标找到对应的地块
            Center center = directGenMap.globalMap.Graph.FindCenter(position);
            // 获取texture
            Texture2D texture = (Texture2D)_showCamp.GetComponent<Renderer>().material.mainTexture;
        }

        private void LocationCamp()
        {
            // 通过坐标找到对应的地块
            Center center = directGenMap.globalMap.Graph.FindCenter(position);
            locationCamp = center.camp;
        }
    }
}

