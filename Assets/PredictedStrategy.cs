// PredictedStrategy.cs
using Newtonsoft.Json;

[System.Serializable]
public class PredictedStrategy
{
    [JsonProperty("����Ӱ��������")]
    public string mostInfluentialFeature;

    [JsonProperty("Ԥ��ļƲ߱��")]
    public float strategyId;

    [JsonProperty("Ԥ��ļƲ�����")]
    public string strategyName;
}