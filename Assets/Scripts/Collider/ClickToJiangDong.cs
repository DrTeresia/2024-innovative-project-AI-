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
        //找到Canvas父类下的Panel父类下的Where子类
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

    //在鼠标进入范围后，将Where子类的text更改为“江东统一战”
    void OnMouseEnter()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "江东统一战";
    }

    //在鼠标离开范围后，将Where子类的text更改为“。。。。。。。。。。”
    void OnMouseExit()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "。。。。。。。。。。";
    }
}
