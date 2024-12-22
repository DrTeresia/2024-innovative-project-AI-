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

    // 在鼠标进入范围后，将Where子类的text更改为“荆州战役”
    void OnMouseEnter()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "荆州战役";
    }

    // 在鼠标离开范围后，将Where子类的text更改为“。。。。。。。。。。”
    void OnMouseExit()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "。。。。。。。。。。";
    }
}
