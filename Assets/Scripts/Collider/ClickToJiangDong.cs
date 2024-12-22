using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToJiangDong : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject targetSceneName;
    void Start()
    {
        //�ҵ�Canvas�����µ�Panel�����µ�Where����
        targetSceneName = GameObject.Find("Canvas/Panel/Where");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene("JiangDongUnifyBattle");
    }

    //�������뷶Χ�󣬽�Where�����text����Ϊ������ͳһս��
    void OnMouseEnter()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "����ͳһս";
    }

    //������뿪��Χ�󣬽�Where�����text����Ϊ������������������������
    void OnMouseExit()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "��������������������";
    }
}
