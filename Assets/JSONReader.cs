// JsonReader.cs
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class JsonReader : MonoBehaviour
{
    void Start()
    {
        // 从Resources文件夹加载JSON文件
        TextAsset jsonFile = Resources.Load<TextAsset>("predicted_strategy");
        if (jsonFile != null)
        {
            // 反序列化JSON
            PredictedStrategy strategy = JsonConvert.DeserializeObject<PredictedStrategy>(jsonFile.text);

            // 打印数据（测试用）
            Debug.Log($"特征: {strategy.mostInfluentialFeature}");
            Debug.Log($"计策ID: {strategy.strategyId}");
            Debug.Log($"计策名称: {strategy.strategyName}");
        }
        else
        {
            Debug.LogError("JSON文件未找到！");
        }
    }
}