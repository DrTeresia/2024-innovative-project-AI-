using System;
using System.Collections.Generic;
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

    [Header("��������")]
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
            if (general != null && general != currentGeneral && currentGeneral == null && general.tag == teamTag)
            {
                // �ҵ��µĽ������л�������ý���
                AssignToGeneral(general);
                break;
            }
        }
    }
    private void HandleAmbushBehavior()
    {
        if (currentGeneral == null) return;

        // ��ȡ������AmbushController
        var generalAmbush = currentGeneral.GetComponent<AmbushController>();
        if (generalAmbush == null) return;

        // �����������״̬ʱ����
        if (generalAmbush.isAmbushing && !isInAmbush)
        {
            StartAmbush(generalAmbush.currentCover);
        }
        // �����˳����״̬ʱ����
        else if (!generalAmbush.isAmbushing && isInAmbush)
        {
            ExitAmbushState();
        }
    }

    private void StartAmbush(Cover generalCover)
    {
        if (generalCover == null) return;

        currentCover = generalCover;
        moveComponent.controlMode = 0; // �л����������ģʽ
        moveComponent.targetPosition = currentCover.transform.position;

        // ������⵽��״̬
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
            // ��������ʱ�ı����߼�
            Debug.Log($"{name}: �������������־���״̬");
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
        moveComponent.controlMode = 1; // �����ƶ�
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
        moveComponent.controlMode = 0; // �ָ��ƶ�
    }
}