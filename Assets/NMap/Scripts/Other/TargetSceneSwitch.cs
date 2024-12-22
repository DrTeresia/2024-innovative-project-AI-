using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TargetSceneSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    //������goToTargetMap��ťʱ��ȡtargetMapName��������룬Ȼ����ת��target map

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
        //����Ŀ�곡������ǰ���������Ѿ���ǰ����
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetMapName));
        SceneManager.UnloadSceneAsync("DirectGenMap");

    }

}
