using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderOnCollision : MonoBehaviour
{
    [Tooltip("需要碰撞的标签（如Player）")]
    public string targetTag = "Player";

    [Tooltip("要加载的场景名称或索引")]
    public string sceneName; // 或者使用 public int sceneIndex;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        // 使用场景名称加载
        SceneManager.LoadScene(sceneName);

        //或者使用场景索引加载
        // SceneManager.LoadScene(sceneIndex);
    }
}