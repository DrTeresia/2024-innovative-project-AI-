//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Pathfinding;

//public class Find_Leader : MonoBehaviour
//{
//    // Start is called before the first frame update
//    public string targetTag; // 用于标记指定标签类型的对象

//    private AIDestinationSetter aiDestinationSetter; // AIDestinationSetter脚本的引用
//    void Start()
//    {
//        // 获取AIDestinationSetter脚本的引用
//        aiDestinationSetter = GetComponent<AIDestinationSetter>();

//        if (aiDestinationSetter == null)
//        {
//            Debug.LogError("AIDestinationSetter script is not attached to the same GameObject.");
//            return;
//        }

//        // 查找具有targetTag标签的对象
//        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);

//        if (targetObject != null)
//        {
//            // 将找到的对象设置为AIDestinationSetter的Target
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
