using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ClickToNorth : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject targetSceneName;

    void Start()
    {
        targetSceneName = GameObject.Find("Canvas/Panel/Where");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // When the object is clicked, the scene will switch to NorthUnifyBattle
    void OnMouseDown()
    {
        SceneManager.LoadScene("NorthUnifyBattle");
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
