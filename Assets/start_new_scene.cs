using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class jumpScenes : MonoBehaviour
{
    public void jumpto_jingzhou(string sceneName)
    {
        SceneManager.LoadScene(3);//�����ı��
    }
    public void jumpto_jiangdong(string sceneName)
    {
        SceneManager.LoadScene(1);
    }
    public void jumpto_North(string sceneName)
    {
        SceneManager.LoadScene(2);
    }
}
