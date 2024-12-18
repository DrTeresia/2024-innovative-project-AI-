//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Mathematics;
//public class chose_role : MonoBehaviour
//{
//    // Start is called before the first frame update
//    private float startPosition;
//    private float endPosition;
    
//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0))      //点击鼠标
//        {
//            startPosition = UtilsClass.GetMouseWorldPosition();     
//        }

//        if (Input.GetMouseButtonUp(0))      //松开鼠标
//        {
//            endPosition = UtilsClass.GetMouseWorldPosition();

//            float lowerLeftPosition = new float(math.min(startPosition.x, endPosition.x), math.min(startPosition.y, endPosition.y), 0);
//            float upperRightPosition = new float(math.max(startPosition.x, endPosition.x), math.max(startPosition.y, endPosition.y), 0);

//            Entityes.ForEach((Entity entity, ref Translation translation) =>
//            {
//                float entityPosition = translation.Value;
//                if (entityPosition.x >= lowerLeftPosition && entityPosition.x <= upperRightPosition.x && entityPosition.y >= lowerLeftPosition.y && entityPosition.y <= upperRightPosition.y)
//                {
//                    Debug.Log(entity);
//                }
//            });
//        }
//    }
//}

