using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    public FormationType currentFormation = FormationType.RandomOffset;
    [SerializeField] private float spacing = 2.0f;
    [SerializeField] private float randomOffset = 0.5f;
    public List<GeneralFollower> soldiers = new List<GeneralFollower>();

    void Start()
    {
        soldiers = FindObjectsOfType<GeneralFollower>().ToList();
        AssignIndices();
    }

    void AssignIndices()
    {
        for (int i = 0; i < soldiers.Count; i++)
        {
            soldiers[i].soldierIndex = i;
        }
    }

    public void SetFormation(FormationType formation)
    {
        currentFormation = formation;
        foreach (var soldier in soldiers)
        {
            soldier.formationType = formation;
        }
    }
}