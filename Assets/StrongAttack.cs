using UnityEngine;
using System.Collections;
using System;

public class StrongAttackController : MonoBehaviour
{
    [Header("Strong Attack Settings")]
    [SerializeField] private string teamTag = "TeamA"; // �Լ�����Ӫ��ǩ
    //[SerializeField] private string enemyTag = "Enemy"; // ���˵ı�ǩ
    [SerializeField] private string[] enemyTags = new string[] { "Enemy" };
    [SerializeField] private float detectionRange = 10f; // ��ⷶΧ

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

    private void OnInputCommandChanged(object command)        //����ǿ��
    {
        string inputCommand = (string)command;

        string[] commandParts = inputCommand.Split(new[] { '��', ':' }, 2, StringSplitOptions.RemoveEmptyEntries); // ֧������/Ӣ��ð��
        if (commandParts.Length != 2)
        {
            //Debug.LogWarning($"ָ���ʽ����: {inputCommand} (��ȷ��ʽ: ���֣�����)");
            return;
        }

        string targetName = commandParts[0].Trim();
        string strategy = commandParts[1].Trim();

        Debug.Log($"�������� - Ŀ��: {targetName}, ����: {strategy}");

        if (targetName == gameObject.name && strategy == "ǿ��")
        {
            Debug.Log($"{gameObject.name} ִ��ǿ��ָ��");
            ExecuteStrongAttack();
        }
    }

    private void ExecuteStrongAttack()
    {
        if (isAttacking)
        {
            Debug.LogWarning("����ǿ��״̬�������ظ�����");
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
            Debug.LogWarning("δ�ҵ�����");
            isAttacking = false;
            yield break;
        }

        moveScript.controlMode = 0;
        // ���Ŀ���Ƿ�����
        if (currentTarget == null || currentTarget.gameObject == null)
        {
            Debug.LogWarning("Ŀ���Ѳ�����");
            isAttacking = false;
            yield break;
        }

        moveScript.targetPosition = currentTarget.position;

        while (true)
        {
            // ���Ŀ���Ƿ�����
            if (currentTarget == null || currentTarget.gameObject == null)
            {
                Debug.LogWarning("Ŀ���Ѳ�����");
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