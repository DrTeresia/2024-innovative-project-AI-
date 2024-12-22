using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsynchronousLoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    List<string> listOfScene = new List<string>();

    //Slider作为进度条
    public Slider slider;
    private AsyncOperation asyncOperation;

    //设定要提前加载的场景
    /*
    mapByName
    NorthUnifyBattle
    JiangdongUnifyBattle
     */

    private void Awake()
    {
        //获取mainSilder
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
            //加载进度
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
