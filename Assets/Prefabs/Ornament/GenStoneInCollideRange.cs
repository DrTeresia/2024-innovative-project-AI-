using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenStoneInCollideRange : MonoBehaviour
{
    //��ȡ��ײ�����, ��ײ��ΪPolygon Collider 2D
    private PolygonCollider2D collider;
    public GameObject boulder;
    public GameObject goldVein;


    void Start()
    {
        //��Polygon Collider 2D ���
        collider = GetComponent<PolygonCollider2D>();
        //��ȡ��ײ��ķ�Χ
        Bounds bounds = collider.bounds;
        //��ȡ��ײ������ĵ�
        Vector3 center = bounds.center;
        //��ȡ��ײ��İ뾶
        Vector3 extents = bounds.extents;

        //����ײ�巶Χ������ʯͷ
        for (int i = 0; i < 20; i++)
        {
            //�������λ��
            Vector3 pos = new Vector3(center.x + Random.Range(-extents.x, extents.x), center.y + Random.Range(-extents.y, extents.y), center.z + Random.Range(-extents.z, extents.z));
            //����ڷ�Χ��������ѡ��λ��
            while (!collider.OverlapPoint(pos))
            {
                pos = new Vector3(center.x + Random.Range(-extents.x, extents.x), center.y + Random.Range(-extents.y, extents.y), center.z + Random.Range(-extents.z, extents.z));
            }
            //����ʯͷ
            Instantiate(boulder, pos, Quaternion.identity);
        }

        //����ײ�巶Χ�����ɽ��
        for (int i = 0; i < 10; i++)
        {
            //�������λ��
            Vector3 pos = new Vector3(center.x + Random.Range(-extents.x, extents.x), center.y + Random.Range(-extents.y, extents.y), center.z + Random.Range(-extents.z, extents.z));
            //����ڷ�Χ��������ѡ��λ��
            while (!collider.OverlapPoint(pos))
            {
                pos = new Vector3(center.x + Random.Range(-extents.x, extents.x), center.y + Random.Range(-extents.y, extents.y), center.z + Random.Range(-extents.z, extents.z));
            }
            //���ɽ��
            Instantiate(goldVein, pos, Quaternion.identity);

        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
