using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreLoadTheScene : MonoBehaviour
{
    //This scripts is used to asynchronously load the scene before the player enters the scene
    //Scene name is JiangdongUnifyBattle/NorthUnityBattle/JinZhouBattle

    public string sceneName;
    void Start()
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
