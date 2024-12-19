using UnityEngine;
using UnityEngine.UI;  // ���� UI �����ռ�

public class Logger : MonoBehaviour
{
    public Text logText;  // �� Inspector �����õ� UI Text ���

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // �����־��Ϣ�Ƿ�����ض��ؼ���
        if (logText != null && ShouldLogMessage(logString))
        {
            logString = logString.Replace("Message Content:", "");
            // ����־��Ϣ׷�ӵ� Text ���
            logText.text += logString + "\n";
        }
    }

    // ����һ�������������־��Ϣ�Ƿ�Ӧ�ñ���¼
    bool ShouldLogMessage(string logString)
    {
        // ������ϣ��������ض����ݻ�ؼ���
        string keyword = "Message Content:"; // �滻Ϊ��ϣ������Ĺؼ��ʻ�����

        // �����־��Ϣ�Ƿ�����ؼ���
        return logString.Contains(keyword);
    }
}