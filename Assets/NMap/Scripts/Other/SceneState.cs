using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ����״̬��������
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
        public string customData; // ����չ���Զ�������
    }

    public List<ObjectState> savedObjects = new List<ObjectState>();
}
