using UnityEngine;
using UnityEngine.UI;  // 引用 UI 命名空间

public class Logger : MonoBehaviour
{
    public Text logText;  // 在 Inspector 中设置的 UI Text 组件

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
        // 检查日志消息是否包含特定关键词
        if (logText != null && ShouldLogMessage(logString))
        {
            logString = logString.Replace("Message Content:", "");
            // 将日志信息追加到 Text 组件
            logText.text += logString + "\n";
        }
    }

    // 定义一个方法来检查日志消息是否应该被记录
    bool ShouldLogMessage(string logString)
    {
        // 定义您希望捕获的特定内容或关键词
        string keyword = "Message Content:"; // 替换为您希望捕获的关键词或条件

        // 检查日志消息是否包含关键词
        return logString.Contains(keyword);
    }
}