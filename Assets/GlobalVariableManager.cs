using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class GlobalVariableManager : MonoBehaviour
{
    public static GlobalVariableManager Instance { get; private set; }

    // 公共变量，可在 Inspector 中直接输入
    [SerializeField] private string _inputCommand;

    // JSON文件配置
    [Header("JSON配置")]
    [SerializeField] private bool loadFromJSON = true;
    [SerializeField] private string strategyFileName = "predicted_strategy.json";

    private Dictionary<string, object> _variables = new Dictionary<string, object>();
    private Dictionary<string, Action<object>> _onVariableChanged = new Dictionary<string, Action<object>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 初始化时同步公共变量到全局字典
        if (loadFromJSON)
        {
            StartCoroutine(LoadStrategyFromJSON());
        }
        else
        {
            SetVariable("inputCommand", _inputCommand);
        }
    }
    private IEnumerator LoadStrategyFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, strategyFileName);

        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonContent = request.downloadHandler.text;
                    List<StrategyEntry> strategyEntries = JsonConvert.DeserializeObject<List<StrategyEntry>>(jsonContent);

                    // 构建命令字符串
                    StringBuilder commandBuilder = new StringBuilder();
                    foreach (var entry in strategyEntries)
                    {
                        commandBuilder.AppendLine($"{entry.名字}: {entry.预测的计策名称}");
                    }

                    string combinedCommand = commandBuilder.ToString().Trim();
                    SetVariable("inputCommand", combinedCommand);
                    Debug.Log($"Successfully loaded strategies:\n{combinedCommand}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON解析错误: {e.Message}");
                    SetVariable("inputCommand", _inputCommand);
                }
            }
            else
            {
                Debug.LogError($"文件加载错误: {request.error}");
                SetVariable("inputCommand", _inputCommand);
            }
        }
    }

    // 当 Inspector 中的值变化时触发（仅在编辑器模式下有效）
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        SetVariable("inputCommand", _inputCommand);
    }

    public void SetVariable(string key, object value)
    {
        if (_variables.ContainsKey(key))
        {
            _variables[key] = value;
        }
        else
        {
            _variables.Add(key, value);
        }
        TriggerEvent(key, value);
    }

    public T GetVariable<T>(string key)
    {
        return _variables.ContainsKey(key) ? (T)_variables[key] : default(T);
    }

    public void Subscribe(string key, Action<object> callback)
    {
        if (!_onVariableChanged.ContainsKey(key))
        {
            _onVariableChanged.Add(key, null);
        }
        _onVariableChanged[key] += callback;
    }

    public void Unsubscribe(string key, Action<object> callback)
    {
        if (_onVariableChanged.ContainsKey(key))
        {
            _onVariableChanged[key] -= callback;
        }
    }

    private void TriggerEvent(string key, object value)
    {
        _onVariableChanged.TryGetValue(key, out Action<object> eventCallback);
        eventCallback?.Invoke(value);
    }

    [System.Serializable]
    private class PredictedStrategy
    {
        public string strategyName;
    }
    [System.Serializable]
    private class StrategyEntry
    {
        public string 名字;
        public string 最有影响力特征;
        public double 预测的计策编号;
        public string 预测的计策名称;
    }
}