using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using System;

public class CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera c;
    private float mouseCenter;
    private int maxView = int.MaxValue;
    private int minView = int.MinValue;
    public float slideSpeed = 5.0f;

    public Vector3 last_frame_mouse_position;
    public float horizontal_speed = 2.0f;
    public float vertical_speed = 2.0f;
    public float horizontal_limit = 80.0f;
    public float vertical_limit = 80.0f;

    public int curOrthographicSize = 4;

    void Start()
    {
        c = this.GetComponent<Camera>();
        last_frame_mouse_position = Input.mousePosition;
        c.transform.position = new Vector3(0, 0, -10);
        // set size as 4
        c.orthographicSize = 4;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseCenter = Input.GetAxis("Mouse ScrollWheel");
        curOrthographicSize = (int)c.orthographicSize;

        if (mouseCenter > 0)
        {
            //ͶӰ����orthographic
            if (c.orthographic)
            {
                c.orthographicSize -= slideSpeed;
                c.orthographicSize = Mathf.Clamp(c.orthographicSize, 1, 100);
            }
        }
        else if (mouseCenter < 0)
        {
            //ͶӰ����orthographic
            if (c.orthographic)
            {
                c.orthographicSize += slideSpeed;
                c.orthographicSize = Mathf.Clamp(c.orthographicSize, 1, 100);
            }
        }
        //������ƶ������������ο�ϵ�����������ת�޹�
        if (Input.GetMouseButtonDown(1))
        {
            last_frame_mouse_position = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - last_frame_mouse_position;
            //������ӽ�Խ���ƶ��ٶ�Խ�죬 �����ӽǴ�С�ı����� ���������קһ��
            //transform.Translate(-delta.x * horizontal_speed * Time.deltaTime, -delta.y * vertical_speed * Time.deltaTime, 0);
            c.transform.Translate(-delta.x * horizontal_speed * Time.deltaTime * c.orthographicSize / 5, -delta.y * vertical_speed * Time.deltaTime * c.orthographicSize / 5, 0);
            //c.transform.position = new Vector3(Mathf.Clamp(c.transform.position.x, -horizontal_limit, horizontal_limit), Mathf.Clamp(c.transform.position.y, -vertical_limit, vertical_limit), c.transform.position.z);
            last_frame_mouse_position = Input.mousePosition;
        }
    }
}
