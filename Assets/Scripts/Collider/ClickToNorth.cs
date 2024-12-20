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

    //在鼠标进入范围后，将Where子类的text更改为“北方统一战”
    void OnMouseEnter()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "北方统一战";
    }

    //在鼠标离开范围后，将Where子类的text更改为“。。。。。。。。。。”
    void OnMouseExit()
    {
        targetSceneName.GetComponent<UnityEngine.UI.Text>().text = "。。。。。。。。。。";
    }

}
