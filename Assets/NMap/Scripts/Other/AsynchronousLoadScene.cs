using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsynchronousLoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    List<string> listOfScene = new List<string>();

    //Slider��Ϊ������
    public Slider slider;
    private AsyncOperation asyncOperation;

    //�趨Ҫ��ǰ���صĳ���
    /*
    mapByName
    NorthUnifyBattle
    JiangdongUnifyBattle
     */

    private void Awake()
    {
        //��ȡmainSilder
        slider = GameObject.Find("mainSlider").GetComponent<Slider>();

        listOfScene.Add("mapByName");
        listOfScene.Add("NorthUnifyBattle");
        listOfScene.Add("JiangdongUnifyBattle");
    }

    IEnumerator LoadScene()
    {
        foreach (string sceneName in listOfScene)
        {
            asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            //���ؽ���
            while (asyncOperation.progress < 0.9f)
            {
                slider.value = asyncOperation.progress;
                yield return null;
            }
        }
    }

    void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
