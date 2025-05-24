using UnityEngine;
using System.Collections;
using System;

public class StrongAttackController : MonoBehaviour
{
    [Header("Strong Attack Settings")]
    [SerializeField] private string teamTag = "TeamA"; // 自己的阵营标签
    [SerializeField] private string enemyTag = "Enemy"; // 敌人的标签
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

    private void OnInputCommandChanged(object command)
    {
        string inputCommand = (string)command;
        string[] commandParts = inputCommand.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

        // 验证命令格式有效性
        if (commandParts.Length != 2) return;

        string targetName = commandParts[0];
        string strategy = commandParts[1];

        // 调试输出解析结果
        Debug.Log($"解析命令 - 目标: {targetName}, 计策: {strategy}");

        // 判断是否执行伏击
        if (targetName == gameObject.name && strategy == "强攻")
        {
            ExecuteStrongAttack();
            Debug.Log($"{gameObject.name} 执行强攻命令");
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
            if (enemy.CompareTag(enemyTag))
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