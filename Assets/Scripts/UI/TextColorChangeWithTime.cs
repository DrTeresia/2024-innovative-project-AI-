using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextColorChangeWithTime : MonoBehaviour
{
    // ��ȡtext���
    private UnityEngine.UI.Text text;

    //��ɫ�仯���ٶ�
    public float speed = 1.0f;

    //��ɫ
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
        // ����ʱ������ƣ�text����ɫ��ƽ���仯
        time += Time.deltaTime * speed;
        text.color = Color.Lerp(color, Color.red, time);

    }
}
