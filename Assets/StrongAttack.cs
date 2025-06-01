using UnityEngine;
using System.Collections;
using System;

public class StrongAttackController : MonoBehaviour
{
    [Header("Strong Attack Settings")]
    [SerializeField] private string teamTag = "TeamA"; // 自己的阵营标签
    //[SerializeField] private string enemyTag = "Enemy"; // 敌人的标签
    [SerializeField] private string[] enemyTags = new string[] { "Enemy" };
    [SerializeField] private float detectionRange = 10f; // 检测范围

    private Move moveScript;
    private Transform currentTarget;
    private bool isAttacking = false;

    void Awake()
    {
        moveScript = GetComponent<Move>();
        GlobalVariableManager.Instance.Subscribe("inputCommand", OnInputCommandChanged);
    }

    void OnDisable()
    {
        GlobalVariableManager.Instance.Unsubscribe("inputCommand", OnInputCommandChanged);
    }

    private void OnInputCommandChanged(object command)        //关羽：强攻
    {
        string inputCommand = (string)command;

        string[] commandParts = inputCommand.Split(new[] { '：', ':' }, 2, StringSplitOptions.RemoveEmptyEntries); // 支持中文/英文冒号
        if (commandParts.Length != 2)
        {
            //Debug.LogWarning($"指令格式错误: {inputCommand} (正确格式: 名字：命令)");
            return;
        }

        string targetName = commandParts[0].Trim();
        string strategy = commandParts[1].Trim();

        Debug.Log($"解析命令 - 目标: {targetName}, 策略: {strategy}");

        if (targetName == gameObject.name && strategy == "强攻")
        {
            Debug.Log($"{gameObject.name} 执行强攻指令");
            ExecuteStrongAttack();
        }
    }

    private void ExecuteStrongAttack()
    {
        if (isAttacking)
        {
            Debug.LogWarning("已在强攻状态，不可重复触发");
            return;
        }

        isAttacking = true;
        StartCoroutine(StrongAttackRoutine());
    }

    private IEnumerator StrongAttackRoutine()
    {
        currentTarget = FindNearestEnemy();
        if (currentTarget == null)
        {
            Debug.LogWarning("未找到敌人");
            isAttacking = false;
            yield break;
        }

        moveScript.controlMode = 0;
        // 检查目标是否被销毁
        if (currentTarget == null || currentTarget.gameObject == null)
        {
            Debug.LogWarning("目标已不存在");
            isAttacking = false;
            yield break;
        }

        moveScript.targetPosition = currentTarget.position;

        while (true)
        {
            // 检查目标是否被销毁
            if (currentTarget == null || currentTarget.gameObject == null)
            {
                Debug.LogWarning("目标已不存在");
                break;
            }

            float distance = Vector2.Distance(transform.position, currentTarget.position);
            if (distance <= 0.5f)
            {
                break;
            }

            yield return null;
        }

        isAttacking = false;
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        Transform nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            bool isEnemy = false;
            foreach (string tag in enemyTags)
            {
                if (enemy.CompareTag(tag))
                {
                    isEnemy = true;
                    break;
                }
            }

            if (isEnemy)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }
        }

        return nearestEnemy;
    }
}