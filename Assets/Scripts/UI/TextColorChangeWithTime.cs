using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextColorChangeWithTime : MonoBehaviour
{
    // 获取text组件
    private UnityEngine.UI.Text text;

    //颜色变化的速度
    public float speed = 1.0f;

    //颜色
    private Color color;
    private float time = 0;


    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<UnityEngine.UI.Text>();
        color = text.color;
    }

    // Update is called once per frame

    
    void Update()
    {
        // 随着时间的推移，text的颜色将平滑变化
        time += Time.deltaTime * speed;
        text.color = Color.Lerp(color, Color.red, time);

    }
}
