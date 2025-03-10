using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Map{
    public class ChangeFieldColor : MonoBehaviour
    {
        // ����������������£�������ﵽ�ض�����ʱ���ĵؿ���ɫ
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

        // �����������ͳһ�ؿ���ͣ������һ��ʱ�䣬�ı�ؿ���ɫ
        private float timer = 0;
        private Center bufferCenter;
        void Update()
        {
            // ͨ�������ҵ���Ӧ�ĵؿ�
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

        // ��������ı�ؿ���ɫ
        public void ChangeColor(Vector2 position, int camp)
        {
            // ͨ�������ҵ���Ӧ�ĵؿ�
            Center center = directGenMap.globalMap.Graph.FindCenter(position);
            // ��ȡtexture
            Texture2D texture = (Texture2D)_showCamp.GetComponent<Renderer>().material.mainTexture;
        }

        private void LocationCamp()
        {
            // ͨ�������ҵ���Ӧ�ĵؿ�
            Center center = directGenMap.globalMap.Graph.FindCenter(position);
            locationCamp = center.camp;
        }
    }
}

