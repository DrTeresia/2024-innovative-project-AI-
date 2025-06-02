using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json; 

public class ArmyManager : MonoBehaviour
{
    public List<General> generals = new List<General>();
    public List<Castle> castles = new List<Castle>();
    public List<General> nearbyGenerals = new List<General>();
    public float detectionRange = 20f; 
    public float interval = 10f; 

    private Dictionary<Soldier, General> soldierToGeneralMap = new Dictionary<Soldier, General>();
    public LayerMask generalLayer;

    private float timer = 0f; // ��ʱ��

    public static ArmyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保持
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // 确保场景中所有将军都被注册
        General[] allGenerals = FindObjectsOfType<General>();
        foreach (General general in allGenerals)
        {
            RegisterGeneral(general);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            WriteGeneralsInfoToJson();
            timer = 0f; 
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

    // 新增方法：检测一定范围内的智能体（General）
    public List<General> DetectNearbyGenerals(Vector2 position)
    {
        List<General> nearbyGenerals = new List<General>();

        foreach (General general in generals)
        {
            if (general != null && Vector2.Distance(position, general.transform.position) <= detectionRange)
            {
                nearbyGenerals.Add(general);
            }
        }

        return nearbyGenerals;
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
            nearbyGenerals = DetectNearbyGenerals(general.transform.position);
            int friendlySoldiers = 1;
            int enemySoldiers = 1;
            foreach (var nearbyGeneral in nearbyGenerals)
            {
                if (nearbyGeneral == null) continue;
                if (nearbyGeneral.tag == general.tag)
                    friendlySoldiers += nearbyGeneral.SoldierCount;
                else
                    enemySoldiers += nearbyGeneral.SoldierCount;
            }

            float soldierComparison = enemySoldiers > 0 ? (float)friendlySoldiers / enemySoldiers : friendlySoldiers;
            generalsInfo.Add(new GeneralInfo
            {
                Name = general.name,
                兵力比归一化 = soldierComparison,
                地形编码 = general.environment,
                天气编码 = general.weather,
                发生时间 = general.happenTime
            });
        }

        string json = JsonConvert.SerializeObject(generalsInfo, Formatting.Indented);
        string directoryPath = Path.Combine(Application.persistentDataPath, "General_Data"); // ʹ�� Path.Combine ȷ��·����ȷ
        string filePath = Path.Combine(directoryPath, "generals_info.json"); // ʹ�� Path.Combine ȷ��·����ȷ

        // ȷ��Ŀ¼����
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
        public float 兵力比归一化;
        public float 地形编码;
        public int 天气编码;
        public int 发生时间;

    }

}