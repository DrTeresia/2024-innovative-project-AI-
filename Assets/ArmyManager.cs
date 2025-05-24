using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json; // ���� JSON.NET ������ JSON ���л�

public class ArmyManager : MonoBehaviour
{
    public List<General> generals = new List<General>();
    public List<Castle> castles = new List<Castle>();
    public float detectionRange = 20f; // ��ⷶΧ
    public float interval = 10f; // ��ʱ������ʱ�䣨�룩

    private Dictionary<Soldier, General> soldierToGeneralMap = new Dictionary<Soldier, General>();
    public LayerMask generalLayer;

    private float timer = 0f; // ��ʱ��

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            WriteGeneralsInfoToJson();
            timer = 0f; // ���ö�ʱ��
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
                �����ȹ�һ�� = general.SoldierCount,
                ���α��� = general.environment,
                �������� = general.weather,
                ����ʱ�� = general.happenTime
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
        public int �����ȹ�һ��;
        public int ���α���;
        public int ��������;
        public int ����ʱ��;

    }

}