using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera c;
    private float mouseCenter;
    private int maxView = int.MaxValue;
    private int minView = int.MinValue;
    private float slideSpeed = 5;

    public Vector3 last_frame_mouse_position;
    public float horizontal_speed = 2.0f;
    public float vertical_speed = 2.0f;

    void Start()
    {
        c = this.GetComponent<Camera>();
        last_frame_mouse_position = Input.mousePosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        float mouseCenter = Input.GetAxis("Mouse ScrollWheel");
        if (mouseCenter > 0)
        {
            if (c.fieldOfView > minView)
            {
                c.fieldOfView -= slideSpeed;
            }
        }
        else if (mouseCenter < 0)
        {
            if (c.fieldOfView < maxView)
            {
                c.fieldOfView += slideSpeed;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            last_frame_mouse_position = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - last_frame_mouse_position;
            transform.Translate(-delta.x * horizontal_speed * Time.deltaTime, -delta.y * vertical_speed * Time.deltaTime, 0);
            last_frame_mouse_position = Input.mousePosition;
        }
    }
}
