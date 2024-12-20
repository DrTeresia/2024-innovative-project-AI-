using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenStoneInCollideRange : MonoBehaviour
{
    //获取碰撞体组件, 碰撞体为Polygon Collider 2D
    private PolygonCollider2D collider;
    public GameObject boulder;
    public GameObject goldVein;


    void Start()
    {
        //绑定Polygon Collider 2D 组件
        collider = GetComponent<PolygonCollider2D>();
        //获取碰撞体的范围
        Bounds bounds = collider.bounds;
        //获取碰撞体的中心点
        Vector3 center = bounds.center;
        //获取碰撞体的半径
        Vector3 extents = bounds.extents;

        //在碰撞体范围内生成石头
        for (int i = 0; i < 20; i++)
        {
            //生成随机位置
            Vector3 pos = new Vector3(center.x + Random.Range(-extents.x, extents.x), center.y + Random.Range(-extents.y, extents.y), center.z + Random.Range(-extents.z, extents.z));
            //如果在范围外则重新选择位置
            while (!collider.OverlapPoint(pos))
            {
                pos = new Vector3(center.x + Random.Range(-extents.x, extents.x), center.y + Random.Range(-extents.y, extents.y), center.z + Random.Range(-extents.z, extents.z));
            }
            //生成石头
            Instantiate(boulder, pos, Quaternion.identity);
        }

        //在碰撞体范围内生成金矿
        for (int i = 0; i < 10; i++)
        {
            //生成随机位置
            Vector3 pos = new Vector3(center.x + Random.Range(-extents.x, extents.x), center.y + Random.Range(-extents.y, extents.y), center.z + Random.Range(-extents.z, extents.z));
            //如果在范围外则重新选择位置
            while (!collider.OverlapPoint(pos))
            {
                pos = new Vector3(center.x + Random.Range(-extents.x, extents.x), center.y + Random.Range(-extents.y, extents.y), center.z + Random.Range(-extents.z, extents.z));
            }
            //生成金矿
            Instantiate(goldVein, pos, Quaternion.identity);

        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
