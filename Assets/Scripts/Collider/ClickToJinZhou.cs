using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToJinZhou : MonoBehaviour
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
    void OnMouseDown()
    {
        SceneManager.LoadScene("JinZhouBattle");
    }

    // �������뷶Χ�󣬽�Where�����text����Ϊ������ս�ۡ�
    void OnMouseEnter()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "����ս��";
    }

    // ������뿪��Χ�󣬽�Where�����text����Ϊ������������������������
    void OnMouseExit()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "��������������������";
    }
}
