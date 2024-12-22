using UnityEngine;
using UnityEngine.UI;

public class FollowDialog : MonoBehaviour
{
    public Transform target; // 要跟随的目标
    public Vector3 offset; // 对话框相对于目标的偏移量
    public Text dialogText; // 对话框文本
    public Image dialogImage; // 对话框图像

    // Update is called once per frame
    void Update()
    {
        // 将对话框的位置设置为目标位置加上偏移量
        transform.position = target.position + offset;
    }

    // 显示对话框内容
    public void ShowDialog(string text, Sprite image)
    {
        dialogText.text = text;
        dialogImage.sprite = image;
        gameObject.SetActive(true);
    }

    // 隐藏对话框
    public void HideDialog()
    {
        gameObject.SetActive(false);
    }
}