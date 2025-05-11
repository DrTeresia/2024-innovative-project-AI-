using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 场景状态数据容器
[CreateAssetMenu(fileName = "SceneState", menuName = "SaveSystem/SceneState")]
public class SceneState : ScriptableObject
{
    [System.Serializable]
    public class ObjectState
    {
        public string objectId;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string customData; // 可扩展的自定义数据
    }

    public List<ObjectState> savedObjects = new List<ObjectState>();
}
