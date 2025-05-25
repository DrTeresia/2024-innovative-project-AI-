using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Map
{
    public class BuildConstruction : MonoBehaviour
    {
        
        public bool isSelected = false;

        // Panel��Ҫ�ֶ���ק
        public GameObject Panel;
        private DirectGenMap directGenMap;
        private ChangeFieldColor changeFieldColor;


        void Start()
        {
            // ���directGenMap�Ƿ�Ϊ��
            directGenMap = Panel.GetComponent<DirectGenMap>();
            if (directGenMap == null)
            {
                Debug.Log("directGenMap is null");
                return;
            }
            // ���changeFieldColor�Ƿ�Ϊ��
            changeFieldColor = GetComponent<ChangeFieldColor>();
            if (changeFieldColor == null)
            {
                Debug.Log("changeFieldColor is null");
                return;
            }
        }
        void Update()
        {
            if (isSelected)
            {
                int selfCamp = changeFieldColor.camp;
                // ���directGenMap�Ƿ�Ϊ��
                if (directGenMap == null)
                {
                    directGenMap = Panel.GetComponent<DirectGenMap>();
                    if (directGenMap == null)
                    {
                        Debug.Log("directGenMap is null");
                        return;
                    }
                }

                // ��鵱ǰ�ؿ��Camp�Ƿ����Լ���Camp��ͬ
                Vector2 position = new Vector2((transform.position.x + 75) * 4, (transform.position.y + 50) * 4);
                Center currentCenter = directGenMap.globalMap.Graph.FindCenter(position);
                //if (currentCenter.property != -1)
                //{
                //    // �����ǰ�ؿ��property������-1����������
                //    Debug.Log("Cannot build here, property not -1");
                //    return;
                //}
                //if (currentCenter != null || currentCenter.camp != selfCamp)
                //{
                //    // �����ǰ�ؿ��Camp���Լ���Camp��ͬ����������
                //    Debug.Log("Cannot build here, different camp");
                //    return;
                //}
                //����Ƿ���Z�������������directGenMap.blockList[4]
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    // ���ĵؿ�����
                    currentCenter.property = 4;
                    // ֱ���ڵؿ������ɽ�����
                    GameObject building = Instantiate(directGenMap.blockList[4], new Vector3(currentCenter.point.x/4-75, currentCenter.point.y/4-50, 0), Quaternion.identity);
                    building.transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
                }
            }
        }
    }
}

