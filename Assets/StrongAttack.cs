using UnityEngine;
using System.Collections;

public class StrongAttackController : MonoBehaviour
{
    [Header("Strong Attack Settings")]
    [SerializeField] private string teamTag = "TeamA"; // �Լ�����Ӫ��ǩ
    [SerializeField] private string enemyTag = "Enemy"; // ���˵ı�ǩ
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

    private void OnInputCommandChanged(object command)
    {
        string inputCommand = (string)command;
        if (inputCommand == "ǿ��")
        {
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
        moveScript.targetPosition = currentTarget.position;

        while (Vector2.Distance(transform.position, currentTarget.position) > 0.5f)
        {
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
