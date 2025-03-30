// JsonReader.cs
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class JsonReader : MonoBehaviour
{
    void Start()
    {
        // ��Resources�ļ��м���JSON�ļ�
        TextAsset jsonFile = Resources.Load<TextAsset>("predicted_strategy");
        if (jsonFile != null)
        {
            // �����л�JSON
            PredictedStrategy strategy = JsonConvert.DeserializeObject<PredictedStrategy>(jsonFile.text);

            // ��ӡ���ݣ������ã�
            Debug.Log($"����: {strategy.mostInfluentialFeature}");
            Debug.Log($"�Ʋ�ID: {strategy.strategyId}");
            Debug.Log($"�Ʋ�����: {strategy.strategyName}");
        }
        else
        {
            Debug.LogError("JSON�ļ�δ�ҵ���");
        }
    }
}