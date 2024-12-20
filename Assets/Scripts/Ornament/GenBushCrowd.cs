using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class GenBushCrowd : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bush1;
    public GameObject bush2;
    void Start()
    {
        //在物体周围生成草丛
        for (int i = 0; i < 20; i++)
        {
            //生成随机位置
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-5, 5), transform.position.z);
            //生成草丛
            if (Random.Range(0, 2) == 0)
            {
                Instantiate(bush1, pos, Quaternion.identity);
            }
            else
            {
                Instantiate(bush2, pos, Quaternion.identity);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
