//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Pathfinding;

//public class Find_Leader : MonoBehaviour
//{
//    // Start is called before the first frame update
//    public string targetTag; // ���ڱ��ָ����ǩ���͵Ķ���

//    private AIDestinationSetter aiDestinationSetter; // AIDestinationSetter�ű�������
//    void Start()
//    {
//        // ��ȡAIDestinationSetter�ű�������
//        aiDestinationSetter = GetComponent<AIDestinationSetter>();

//        if (aiDestinationSetter == null)
//        {
//            Debug.LogError("AIDestinationSetter script is not attached to the same GameObject.");
//            return;
//        }

//        // ���Ҿ���targetTag��ǩ�Ķ���
//        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);

//        if (targetObject != null)
//        {
//            // ���ҵ��Ķ�������ΪAIDestinationSetter��Target
//            aiDestinationSetter.Target = targetObject;
//        }
//        else
//        {
//            Debug.LogError("No object found with tag: " + targetTag);
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}
