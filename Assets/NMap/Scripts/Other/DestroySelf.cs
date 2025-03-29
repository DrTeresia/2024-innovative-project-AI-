using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //在0.5s后将物体销毁
        Destroy(gameObject, 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
