using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderOnCollision : MonoBehaviour
{
    [Tooltip("��Ҫ��ײ�ı�ǩ����Player��")]
    public string targetTag = "Player";

    [Tooltip("Ҫ���صĳ������ƻ�����")]
    public string sceneName; // ����ʹ�� public int sceneIndex;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        // ʹ�ó������Ƽ���
        SceneManager.LoadScene(sceneName);

        //����ʹ�ó�����������
        // SceneManager.LoadScene(sceneIndex);
    }
}