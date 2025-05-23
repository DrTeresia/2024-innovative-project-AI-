using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Map
{
    public class BuildConstruction : MonoBehaviour
    {
        
        public bool isSelected = false;

        // Panel需要手动拖拽
        public GameObject Panel;
        private DirectGenMap directGenMap;
        private ChangeFieldColor changeFieldColor;


        void Start()
        {
            // 检查directGenMap是否为空
            directGenMap = Panel.GetComponent<DirectGenMap>();
            if (directGenMap == null)
            {
                Debug.Log("directGenMap is null");
                return;
            }
            // 检查changeFieldColor是否为空
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
                // 检查directGenMap是否为空
                if (directGenMap == null)
                {
                    directGenMap = Panel.GetComponent<DirectGenMap>();
                    if (directGenMap == null)
                    {
                        Debug.Log("directGenMap is null");
                        return;
                    }
                }

                // 检查当前地块的Camp是否与自己的Camp相同
                Vector2 position = new Vector2((transform.position.x + 75) * 4, (transform.position.y + 50) * 4);
                Center currentCenter = directGenMap.globalMap.Graph.FindCenter(position);
                //if (currentCenter.property != -1)
                //{
                //    // 如果当前地块的property不等于-1，则不允许建造
                //    Debug.Log("Cannot build here, property not -1");
                //    return;
                //}
                //if (currentCenter != null || currentCenter.camp != selfCamp)
                //{
                //    // 如果当前地块的Camp与自己的Camp不同，则不允许建造
                //    Debug.Log("Cannot build here, different camp");
                //    return;
                //}
                //检测是否按下Z键，如果是则建造directGenMap.blockList[4]
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    // 更改地块属性
                    currentCenter.property = 4;
                    // 直接在地块上生成建筑物
                    GameObject building = Instantiate(directGenMap.blockList[4], new Vector3(currentCenter.point.x/4-75, currentCenter.point.y/4-50, 0), Quaternion.identity);
                    building.transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
                }
            }
        }
    }
}

