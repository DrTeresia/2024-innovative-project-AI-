using UnityEngine;
using UnityEngine.UI;

public class FollowDialog : MonoBehaviour
{
    public Transform target; // Ҫ�����Ŀ��
    public Vector3 offset; // �Ի��������Ŀ���ƫ����
    public Text dialogText; // �Ի����ı�
    public Image dialogImage; // �Ի���ͼ��

    // Update is called once per frame
    void Update()
    {
        // ���Ի����λ������ΪĿ��λ�ü���ƫ����
        transform.position = target.position + offset;
    }

    // ��ʾ�Ի�������
    public void ShowDialog(string text, Sprite image)
    {
        dialogText.text = text;
        dialogImage.sprite = image;
        gameObject.SetActive(true);
    }

    // ���ضԻ���
    public void HideDialog()
    {
        gameObject.SetActive(false);
    }
}