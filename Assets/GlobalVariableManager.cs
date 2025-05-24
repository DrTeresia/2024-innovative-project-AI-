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

    // �������������� Inspector ��ֱ������
    [SerializeField] private string _inputCommand;

    // JSON�ļ�����
    [Header("JSON����")]
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

        // ��ʼ��ʱͬ������������ȫ���ֵ�
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

                    // ���������ַ���
                    StringBuilder commandBuilder = new StringBuilder();
                    foreach (var entry in strategyEntries)
                    {
                        commandBuilder.AppendLine($"{entry.����}: {entry.Ԥ��ļƲ�����}");
                    }

                    string combinedCommand = commandBuilder.ToString().Trim();
                    SetVariable("inputCommand", combinedCommand);
                    Debug.Log($"Successfully loaded strategies:\n{combinedCommand}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON��������: {e.Message}");
                    SetVariable("inputCommand", _inputCommand);
                }
            }
            else
            {
                Debug.LogError($"�ļ����ش���: {request.error}");
                SetVariable("inputCommand", _inputCommand);
            }
        }
    }

    // �� Inspector �е�ֵ�仯ʱ���������ڱ༭��ģʽ����Ч��
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
        public string ����;
        public string ����Ӱ��������;
        public double Ԥ��ļƲ߱��;
        public string Ԥ��ļƲ�����;
    }
}