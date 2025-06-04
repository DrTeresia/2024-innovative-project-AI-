using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections;

public class GlobalVariableManager : MonoBehaviour
{
    public static GlobalVariableManager Instance { get; private set; }

    [SerializeField] private string _inputCommand;

    // JSON监控相关变量
    private string _strategyFilePath;
    private DateTime _lastModifiedTime;
    private HashSet<string> _processedHashes = new HashSet<string>();

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

        // 初始化JSON监控
        _strategyFilePath = Path.Combine(Application.streamingAssetsPath, "predicted_strategy.json");
        if (File.Exists(_strategyFilePath))
        {
            _lastModifiedTime = File.GetLastWriteTime(_strategyFilePath);
            LoadFromFileSystem(_strategyFilePath);
        }
        StartCoroutine(CheckForFileUpdates());

        SetVariable("inputCommand", _inputCommand);
    }

    private IEnumerator CheckForFileUpdates()
    {
        while (true)
        {
            if (File.Exists(_strategyFilePath))
            {
                DateTime currentModifiedTime = File.GetLastWriteTime(_strategyFilePath);
                if (currentModifiedTime != _lastModifiedTime)
                {
                    _lastModifiedTime = currentModifiedTime;
                    LoadFromFileSystem(_strategyFilePath);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void LoadFromFileSystem(string path)
    {
        try
        {
            string jsonData = File.ReadAllText(path);
            ProcessJSON(jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError($"文件读取失败: {e.Message}");
        }
    }

    private void ProcessJSON(string jsonString)
    {
        try
        {
            string wrappedJson = $"{{\"strategies\":{jsonString}}}";
            var wrapper = JsonUtility.FromJson<StrategyDataWrapper>(wrappedJson);

            foreach (var strategy in wrapper.strategies)
            {
                //string hash = GenerateDataHash(strategy);
                //if (!_processedHashes.Contains(hash))
                //{
                    // 更新inputCommand为最新条目的计策名称
                    _inputCommand = strategy.名字 + ":" + strategy.预测的计策名称;
                    SetVariable("inputCommand", _inputCommand);

                    //_processedHashes.Add(hash);
                    Debug.Log($"更新全局命令: {_inputCommand}");
                    Debug.Log(1);
                //}
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON解析错误: {e.Message}");
        }
    }

    private string GenerateDataHash(StrategyData data)
    {
        string json = JsonUtility.ToJson(data);
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    // 原有功能保持不变
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
    private class StrategyDataWrapper
    {
        public List<StrategyData> strategies;
    }

    [System.Serializable]
    public class StrategyData
    {
        public string 名字;
        public string 最有影响力特征;
        public float 预测的计策编号;
        public string 预测的计策名称;
    }
}