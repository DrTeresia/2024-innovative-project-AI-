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
        battleDataList.Add(new BattleData("崤之战（春秋）", 1, 0.5f, 10, 0.0f, 1, "伏击"));
        battleDataList.Add(new BattleData("马陵之战（战国）", 1, 0.5555555555555556f, 22, 2.0f, 0, "诱敌"));
        battleDataList.Add(new BattleData("巨鹿之战（秦末）", 1, 0.2f, 13, 1.0f, 1, "死战"));
        battleDataList.Add(new BattleData("井陉之战（楚汉）", 1, 0.1304347826086956f, 8, 4.0f, 1, "背水阵"));
        battleDataList.Add(new BattleData("定军山之战（三国）", 1, 0.8823529411764706f, 8, 0.0f, 1, "高地战"));
    }

    void CreateCSV()
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string csvContent = "战役名称,进攻防守,兵力比归一化,地形编码,天气编码,发生时间,使用的计策\n"; // 表头

        foreach (var data in battleDataList)
        {
            csvContent += $"{data.CampaignName},{data.AttackDefense},{data.TroopRatioNormalized}," +
                         $"{data.TerrainCode},{data.WeatherCode},{data.OccurrenceTime},{data.Strategy}\n";
        }

        File.WriteAllText(filePath, csvContent);

        Debug.Log($"CSV 文件已成功创建: {filePath}");
    }

    void UpdateCSV()
    {
        UpdateSampleData();

        string csvContent = "战役名称,进攻防守,兵力比归一化,地形编码,天气编码,发生时间,使用的计策\n";

        foreach (var data in battleDataList)
        {
            csvContent += $"{data.CampaignName},{data.AttackDefense},{data.TroopRatioNormalized}," +
                         $"{data.TerrainCode},{data.WeatherCode},{data.OccurrenceTime},{data.Strategy}\n";
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

    public BattleData(string campaignName, int attackDefense, float troopRatioNormalized,
                     int terrainCode, float weatherCode, int occurrenceTime, string strategy)
    {
        CampaignName = campaignName;
        AttackDefense = attackDefense;
        TroopRatioNormalized = troopRatioNormalized;
        TerrainCode = terrainCode;
        WeatherCode = weatherCode;
        OccurrenceTime = occurrenceTime;
        Strategy = strategy;
    }
}