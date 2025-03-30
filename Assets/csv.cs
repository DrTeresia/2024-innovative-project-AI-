using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVUpdater : MonoBehaviour
{
    public string filePath = "Assets/BattleData.csv"; // CSV�ļ�·��
    public float updateInterval = 10.0f; // ���¼�����룩

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
        battleDataList.Add(new BattleData("��֮ս�����", 1, 0.5f, 10, 0.0f, 1, "����"));
        battleDataList.Add(new BattleData("����֮ս��ս����", 1, 0.5555555555555556f, 22, 2.0f, 0, "�յ�"));
        battleDataList.Add(new BattleData("��¹֮ս����ĩ��", 1, 0.2f, 13, 1.0f, 1, "��ս"));
        battleDataList.Add(new BattleData("����֮ս��������", 1, 0.1304347826086956f, 8, 4.0f, 1, "��ˮ��"));
        battleDataList.Add(new BattleData("����ɽ֮ս��������", 1, 0.8823529411764706f, 8, 0.0f, 1, "�ߵ�ս"));
    }

    void CreateCSV()
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string csvContent = "ս������,��������,�����ȹ�һ��,���α���,��������,����ʱ��,ʹ�õļƲ�\n"; // ��ͷ

        foreach (var data in battleDataList)
        {
            csvContent += $"{data.CampaignName},{data.AttackDefense},{data.TroopRatioNormalized}," +
                         $"{data.TerrainCode},{data.WeatherCode},{data.OccurrenceTime},{data.Strategy}\n";
        }

        File.WriteAllText(filePath, csvContent);

        Debug.Log($"CSV �ļ��ѳɹ�����: {filePath}");
    }

    void UpdateCSV()
    {
        UpdateSampleData();

        string csvContent = "ս������,��������,�����ȹ�һ��,���α���,��������,����ʱ��,ʹ�õļƲ�\n";

        foreach (var data in battleDataList)
        {
            csvContent += $"{data.CampaignName},{data.AttackDefense},{data.TroopRatioNormalized}," +
                         $"{data.TerrainCode},{data.WeatherCode},{data.OccurrenceTime},{data.Strategy}\n";
        }

        File.WriteAllText(filePath, csvContent);

        Debug.Log($"CSV �ļ��ѳɹ�����: {filePath}");
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