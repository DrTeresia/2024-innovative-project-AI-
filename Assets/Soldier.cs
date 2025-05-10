using System;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public ArmyManager armyManager;
    public Move moveComponent; // ����Move�ű�
    public float generalDetectionRange = 20f;

    public General currentGeneral;
    private Vector2 targetPosition;
    private bool isFollowingGeneral = true;
    private Vector2 formationOffset; // ����formationOffset

    private void Start()
    {
        if (armyManager == null)
        {
            armyManager = FindObjectOfType<ArmyManager>();
        }

        if (moveComponent == null)
        {
            moveComponent = GetComponent<Move>();
        }

        // ��ʼ״̬�¿���û�н���������ֱ��Ѱ�ҽ�����ǳ�
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
 

        // ���ƶ���������ʱ����Ƿ��и����Ľ���
        CheckForNearbyGeneral();
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
        Vector2 targetPos = generalPos + formationOffset; // ʹ��formationOffset

        Console.WriteLine(targetPos);

        // ��Ŀ��λ�ô��ݸ�Move�ű�
        moveComponent.targetPosition = targetPos;

        // ��齫���Ƿ��ڷ�Χ��
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
            // ��Ŀ��λ�ô��ݸ�Move�ű�
            moveComponent.targetPosition = targetPosition;

            // ����Ŀ��λ�ú�����Ѱ�ҽ�����ǳ�
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
            // û���ҵ�������ǳأ�������������߼�����Ѳ�ߵ�
            Debug.LogWarning("No general or castle found for soldier");
        }
    }

    private void AssignToGeneral(General general)
    {
        currentGeneral = general;
        isFollowingGeneral = true;
        armyManager.AssignSoldierToGeneral(this, general);
        // ȷ���ڷ��䵽����ʱ����formationOffset
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

        // �����Χ�Ƿ��н���
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, generalDetectionRange, armyManager.generalLayer);
        foreach (Collider2D collider in colliders)
        {
            General general = collider.GetComponent<General>();
            if (general != null && general != currentGeneral)
            {
                // �ҵ��µĽ������л�������ý���
                AssignToGeneral(general);
                break;
            }
        }
    }
}