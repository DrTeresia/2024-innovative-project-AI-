using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideDetect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //��⵽��ײʱ�����ײ������
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider.name);
    }
    void OnCollisionStay(Collision collision)
    {
        Debug.Log("OnCollisionStay");
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("OnCollisionStay2D");
    }
}
