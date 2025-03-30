using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVUpdater : MonoBehaviour
{
    public string filePath = "Assets/BattleData.csv"; // CSV文件路径
    public float updateInterval = 10.0f; // 更新间隔（秒）

    private List<BattleData> battleDataList = new List<BattleData>();
    private float timer = 0.0f;

    void Start()
    {
        InitializeSampleData();
        CreateCSV();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            UpdateCSV();
            timer = 0.0f;
        }
    }

    void InitializeSampleData()
    {
        battleDataList.Add(new BattleData(0.5f, 10, 0.0f, 1));
    }

    void CreateCSV()
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string csvContent = "兵力比归一化,地形编码,天气编码,发生时间（白天1）\n"; // 表头

        foreach (var data in battleDataList)
        {
            csvContent += $"{data.TroopRatioNormalized}," +
                         $"{data.TerrainCode},{data.WeatherCode},{data.OccurrenceTime}\n";
        }

        File.WriteAllText(filePath, csvContent);

        Debug.Log($"CSV 文件已成功创建: {filePath}");
    }

    void UpdateCSV()
    {
        UpdateSampleData();

        string csvContent = "兵力比归一化,地形编码,天气编码,发生时间（白天1）\n";

        foreach (var data in battleDataList)
        {
            csvContent += $"{data.TroopRatioNormalized}," +
                         $"{data.TerrainCode},{data.WeatherCode},{data.OccurrenceTime}\n";
        }

        File.WriteAllText(filePath, csvContent);

        Debug.Log($"CSV 文件已成功更新: {filePath}");
    }

    void UpdateSampleData()
    {
        foreach (var data in battleDataList)
        {
            data.TroopRatioNormalized = Random.Range(0.1f, 0.9f);
            data.WeatherCode = Random.Range(0.0f, 10.0f);
        }
    }
}

[System.Serializable]
public class BattleData
{
    public string CampaignName;
    public int AttackDefense;
    public float TroopRatioNormalized;
    public int TerrainCode;
    public float WeatherCode;
    public int OccurrenceTime;
    public string Strategy;

    public BattleData( float troopRatioNormalized,
                     int terrainCode, float weatherCode, int occurrenceTime)
    {
        TroopRatioNormalized = troopRatioNormalized;
        TerrainCode = terrainCode;
        WeatherCode = weatherCode;
        OccurrenceTime = occurrenceTime;
    }
}