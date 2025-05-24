using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json; // 引入 JSON.NET 库用于 JSON 序列化

public class ArmyManager : MonoBehaviour
{
    public List<General> generals = new List<General>();
    public List<Castle> castles = new List<Castle>();
    public float detectionRange = 20f; // 侦测范围
    public float interval = 10f; // 定时任务间隔时间（秒）

    private Dictionary<Soldier, General> soldierToGeneralMap = new Dictionary<Soldier, General>();
    public LayerMask generalLayer;

    private float timer = 0f; // 定时器

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            WriteGeneralsInfoToJson();
            timer = 0f; // 重置定时器
        }
    }

    public void RegisterGeneral(General general)
    {
        if (!generals.Contains(general))
        {
            generals.Add(general);
        }
    }

    public void UnregisterGeneral(General general)
    {
        if (generals.Contains(general))
        {
            generals.Remove(general);
        }
    }

    public void RegisterCastle(Castle castle)
    {
        if (!castles.Contains(castle))
        {
            castles.Add(castle);
        }
    }

    public General FindNearestGeneral(Vector2 position)
    {
        General nearestGeneral = null;
        float minDistance = float.MaxValue;

        foreach (General general in generals)
        {
            float distance = Vector2.Distance(position, general.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestGeneral = general;
            }
        }

        return nearestGeneral;
    }

    public Castle FindNearestCastle(Vector2 position)
    {
        Castle nearestCastle = null;
        float minDistance = float.MaxValue;

        foreach (Castle castle in castles)
        {
            float distance = Vector2.Distance(position, castle.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCastle = castle;
            }
        }
        Debug.Log(nearestCastle);
        return nearestCastle;
    }

    public void AssignSoldierToGeneral(Soldier soldier, General general)
    {
        soldierToGeneralMap[soldier] = general;
        general.AddSoldierToFormation(soldier);
    }

    public void RemoveSoldierFromGeneral(Soldier soldier)
    {
        if (soldierToGeneralMap.ContainsKey(soldier))
        {
            General general = soldierToGeneralMap[soldier];
            general.RemoveSoldierFromFormation(soldier);
            soldierToGeneralMap.Remove(soldier);
        }
    }

    public General GetSoldierGeneral(Soldier soldier)
    {
        if (soldierToGeneralMap.ContainsKey(soldier))
        {
            return soldierToGeneralMap[soldier];
        }
        return null;
    }

    private void WriteGeneralsInfoToJson()
    {
        List<GeneralInfo> generalsInfo = new List<GeneralInfo>();

        foreach (var general in generals)
        {
            generalsInfo.Add(new GeneralInfo
            {
                Name = general.name,
                兵力比归一化 = general.SoldierCount,
                地形编码 = general.environment,
                天气编码 = general.weather,
                发生时间 = general.happenTime
            });
        }

        string json = JsonConvert.SerializeObject(generalsInfo, Formatting.Indented);
        string directoryPath = Path.Combine(Application.persistentDataPath, "General_Data"); // 使用 Path.Combine 确保路径正确
        string filePath = Path.Combine(directoryPath, "generals_info.json"); // 使用 Path.Combine 确保路径正确

        // 确保目录存在
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("General information written to JSON file: " + filePath);
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.LogError("Access denied when trying to write to file: " + filePath);
            Debug.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error writing to file: " + filePath);
            Debug.LogError(ex.Message);
        }
    }

    [Serializable]
    private class GeneralInfo
    {
        public string Name;
        public int 兵力比归一化;
        public int 地形编码;
        public int 天气编码;
        public int 发生时间;

    }

}