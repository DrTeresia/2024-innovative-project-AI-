using System.Collections.Generic;
using UnityEngine;

public class General : MonoBehaviour
{
    public ArmyManager armyManager;
    public Formation formation = Formation.Rectangle;
    public int soldiersPerRow = 5;
    public float spacing = 1f;

    private List<Soldier> soldiers = new List<Soldier>();
    private Vector2[] formationOffsets;

    public int SoldierCount = 0;
    public float ledRange = 20f;
    public LayerMask soldierLayer; // 检测"Soldier"层

    private void Start()
    {
        if (armyManager != null)
        {
            armyManager.RegisterGeneral(this);
        }

        InitializeFormation();
    }

    private void Update()
    {
        UpdateLedSoldierCount();
    }

    private void OnDestroy()
    {
        if (armyManager != null)
        {
            armyManager.UnregisterGeneral(this);
        }
    }

    private void InitializeFormation()
    {
        CalculateFormationOffsets();
    }

    public void AddSoldierToFormation(Soldier soldier)
    {
        soldiers.Add(soldier);
        UpdateSoldierPositions();
    }

    public void RemoveSoldierFromFormation(Soldier soldier)
    {
        soldiers.Remove(soldier);
        UpdateSoldierPositions();
    }

    private void UpdateLedSoldierCount()
    {
        SoldierCount = 0;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            ledRange,
            soldierLayer
        );

        foreach (Collider2D col in colliders)
        {
            Soldier soldier = col.GetComponent<Soldier>();
            if (soldier != null && soldier.currentGeneral == this)
            {
                SoldierCount++;
            }
        }
    }

    public void UpdateSoldierPositions()
    {
        if (soldiers.Count == 0)
        {
            return;
        }

        CalculateFormationOffsets();

        for (int i = 0; i < soldiers.Count; i++)
        {
            if (i < formationOffsets.Length)
            {
                soldiers[i].SetFormationOffset(formationOffsets[i]);
            }
            else
            {
                // 如果士兵数量超过阵型容量，可以考虑添加额外的逻辑
                soldiers[i].SetFormationOffset(Vector2.zero);
            }
        }
    }

    private void CalculateFormationOffsets()
    {
        formationOffsets = new Vector2[soldiersPerRow * soldiersPerRow];

        switch (formation)
        {
            case Formation.Rectangle:
                CalculateRectangleFormation();
                break;
                // 可以添加其他阵型逻辑
        }
    }

    private void CalculateRectangleFormation()
    {
        int index = 0;
        int rows = Mathf.CeilToInt((float)soldiersPerRow * soldiersPerRow / soldiersPerRow);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < soldiersPerRow; j++)
            {
                if (index >= formationOffsets.Length)
                    break;

                formationOffsets[index] = new Vector2(
                    j * spacing - (soldiersPerRow - 1) * spacing * 0.5f,
                    i * spacing - (rows - 1) * spacing * 0.5f
                );

                index++;
            }
        }
    }
    public void SetFormationOffsetForSoldier(Soldier soldier)
    {
        // 计算士兵在阵型中的偏移量
        // 这里只是一个简单的示例，实际逻辑会根据阵型类型和士兵数量来确定
        int index = soldiers.IndexOf(soldier);
        int soldiersPerRow = 5; // 每行的士兵数量
        int row = index / soldiersPerRow;
        int col = index % soldiersPerRow;
        float spacing = 1f; // 士兵之间的间距

        Vector2 offset = new Vector2(
            col * spacing - (soldiersPerRow - 1) * spacing * 0.5f,
            -row * spacing
        );

        soldier.SetFormationOffset(offset);
    }

    public enum Formation
    {
        Rectangle,
        Triangle,
        Circle
    }
}