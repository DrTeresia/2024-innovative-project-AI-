using System;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public ArmyManager armyManager;
    public Move moveComponent; // 引用Move脚本
    public float generalDetectionRange = 20f;

    public General currentGeneral;
    private Vector2 targetPosition;
    private bool isFollowingGeneral = true;
    private Vector2 formationOffset; // 定义formationOffset

    [Header("伏击设置")]
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();
    [SerializeField] public int hiddenLayer = 9;
    public int originalLayer = 13;
    private Cover currentCover;
    private bool isInAmbush;
    private Dictionary<Behaviour, bool> componentStates = new Dictionary<Behaviour, bool>();

    [SerializeField] private string teamTag;

    private void Start()
    {
        originalLayer = gameObject.layer;
        hiddenLayer = 9;
        teamTag = this.tag;

        if (armyManager == null)
        {
            armyManager = FindObjectOfType<ArmyManager>();
        }

        if (moveComponent == null)
        {
            moveComponent = GetComponent<Move>();
        }

        // 初始状态下可能没有将军，所以直接寻找将军或城池
        if (currentGeneral == null)
        {
            CheckForGeneralOrCastle();
        }
    }

    private void Update()
    {
        if (isFollowingGeneral)
        {
            FollowGeneral();
        }
        else if (!isFollowingGeneral && targetPosition != Vector2.zero)
        {
            //Debug.Log(targetPosition);
            MoveToTarget();
        }


        // 在移动过程中随时检查是否有附近的将军
        CheckForNearbyGeneral();
        HandleAmbushBehavior();
    }

    private void FollowGeneral()
    {
        if (currentGeneral == null)
        {
            isFollowingGeneral = false;
            CheckForGeneralOrCastle();
            return;
        }

        Vector2 generalPos = currentGeneral.transform.position;
        Vector2 targetPos = generalPos + formationOffset; // 使用formationOffset

        Console.WriteLine(targetPos);

        // 将目标位置传递给Move脚本
        moveComponent.targetPosition = targetPos;

        // 检查将军是否还在范围内
        if (Vector2.Distance(transform.position, generalPos) > generalDetectionRange)
        {
            armyManager.RemoveSoldierFromGeneral(this);
            currentGeneral = null;
            isFollowingGeneral = false;
            CheckForGeneralOrCastle();
        }
    }

    private void MoveToTarget()
    {
        if (targetPosition != Vector2.zero)
        {
            // 将目标位置传递给Move脚本
            moveComponent.targetPosition = targetPosition;

            // 到达目标位置后重新寻找将军或城池
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                targetPosition = Vector2.zero;
                CheckForGeneralOrCastle();
            }
        }
    }

    public void SetFormationOffset(Vector2 offset)
    {
        formationOffset = offset;
    }

    private void CheckForGeneralOrCastle()
    {
        if (armyManager == null)
        {
            return;
        }

        General nearestGeneral = armyManager.FindNearestGeneral(transform.position);
        Castle nearestCastle = armyManager.FindNearestCastle(transform.position);

        if (nearestGeneral != null)
        {
            AssignToGeneral(nearestGeneral);
        }
        else if (nearestCastle != null)
        {
            targetPosition = nearestCastle.transform.position;
        }
        else
        {
            // 没有找到将军或城池，可以添加其他逻辑，如巡逻等
            Debug.LogWarning("No general or castle found for soldier");
        }
    }

    private void AssignToGeneral(General general)
    {
        currentGeneral = general;
        isFollowingGeneral = true;
        armyManager.AssignSoldierToGeneral(this, general);
        // 确保在分配到将军时设置formationOffset
        if (general != null)
        {
            general.SetFormationOffsetForSoldier(this);
        }
    }

    private void CheckForNearbyGeneral()
    {
        if (armyManager == null)
        {
            return;
        }

        // 检查周围是否有将军
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, generalDetectionRange, armyManager.generalLayer);
        foreach (Collider2D collider in colliders)
        {
            General general = collider.GetComponent<General>();
            if (general != null && general != currentGeneral && currentGeneral == null && general.tag == teamTag)
            {
                // 找到新的将军，切换到跟随该将军
                AssignToGeneral(general);
                break;
            }
        }
    }
    private void HandleAmbushBehavior()
    {
        if (currentGeneral == null) return;

        // 获取将军的AmbushController
        var generalAmbush = currentGeneral.GetComponent<AmbushController>();
        if (generalAmbush == null) return;

        // 将军进入埋伏状态时触发
        if (generalAmbush.isAmbushing && !isInAmbush)
        {
            StartAmbush(generalAmbush.currentCover);
        }
        // 将军退出埋伏状态时触发
        else if (!generalAmbush.isAmbushing && isInAmbush)
        {
            ExitAmbushState();
        }
    }

    private void StartAmbush(Cover generalCover)
    {
        if (generalCover == null) return;

        currentCover = generalCover;
        moveComponent.controlMode = 0; // 切换到坐标控制模式
        moveComponent.targetPosition = currentCover.transform.position;

        // 持续检测到达状态
        if (Vector2.Distance(transform.position, currentCover.transform.position) <= currentCover.occupyRadius)
        {
            TryOccupyCover();
        }
    }

    private void TryOccupyCover()
    {
        if (currentCover.TryOccupyCover())
        {
            EnterAmbushState();
        }
        else
        {
            // 掩体已满时的备用逻辑
            Debug.Log($"{name}: 掩体已满，保持警戒状态");
            moveComponent.targetPosition = currentCover.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2f;
        }
    }

    private void EnterAmbushState()
    {
        isInAmbush = true;
        componentStates.Clear();

        foreach (var component in componentsToDisable)
        {
            if (component != null)
            {
                componentStates[component] = component.enabled;
                component.enabled = false;
            }
        }
        Debug.Log(111);
        gameObject.layer = hiddenLayer;
        GetComponent<Collider2D>().enabled = false;
        moveComponent.controlMode = 1; // 锁定移动
    }

    public void ExitAmbushState()
    {
        if (!isInAmbush) return;

        foreach (var component in componentsToDisable)
        {
            if (component != null && componentStates.ContainsKey(component))
                component.enabled = componentStates[component];
        }

        gameObject.layer = originalLayer;
        GetComponent<Collider2D>().enabled = true;
        currentCover.ReleaseCover();
        isInAmbush = false;
        moveComponent.controlMode = 0; // 恢复移动
    }
}