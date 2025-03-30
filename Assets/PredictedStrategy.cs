// PredictedStrategy.cs
using Newtonsoft.Json;

[System.Serializable]
public class PredictedStrategy
{
    [JsonProperty("最有影响力特征")]
    public string mostInfluentialFeature;

    [JsonProperty("预测的计策编号")]
    public float strategyId;

    [JsonProperty("预测的计策名称")]
    public string strategyName;
}