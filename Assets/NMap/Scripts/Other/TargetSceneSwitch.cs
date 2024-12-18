using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TargetSceneSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    //当按下goToTargetMap按钮时获取targetMapName对象的输入，然后跳转到target map

    private string targetMapName;
    private Button goToTargetMap;
    private InputField targetMapNameInputField;

    void Start()
    {
        targetMapNameInputField = transform.Find("targetMapName").GetComponent<InputField>();
        goToTargetMap = transform.Find("goToTargetMap").GetComponent<Button>();
        goToTargetMap.onClick.AddListener(GoToTargetMap);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GoToTargetMap();
        }
    }

    private void GoToTargetMap()
    {
        targetMapName = targetMapNameInputField.text;
        //激活目标场景，并前往，场景已经提前加载
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetMapName));
        SceneManager.UnloadSceneAsync("DirectGenMap");

    }

}
