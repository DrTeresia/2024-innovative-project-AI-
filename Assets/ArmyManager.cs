using System;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager : MonoBehaviour
{
    public List<General> generals = new List<General>();
    public List<Castle> castles = new List<Castle>();
    public float detectionRange = 20f; // ¼ì²â½«¾üµÄ·¶Î§

    private Dictionary<Soldier, General> soldierToGeneralMap = new Dictionary<Soldier, General>();
    public LayerMask generalLayer; 

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
}