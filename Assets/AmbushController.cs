using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class AmbushController : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private string enemyTeamTag = "TeamA";
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();
    [SerializeField] private int originalLayer;         // ��¼ԭʼ�㼶
    [SerializeField] private int hiddenLayer;           // ���ʱ�л��������ز㼶

    [Header("�Ʋ�")]
    //public string inputCommand; 

    private Move moveScript;
    public Cover currentCover;
    public bool isAmbushing;
    private Dictionary<Behaviour, bool> componentStates = new Dictionary<Behaviour, bool>();
    private bool isStrategyLoaded = false; // ��������״̬��ʶ

    void Awake()
    {
        moveScript = GetComponent<Move>();
        originalLayer = gameObject.layer;
        //Debug.Log("AmbushController Awake method called");
    }
    void OnEnable()
    {
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

        // ��֤�����ʽ��Ч��
        if (commandParts.Length != 2) return;

        string targetName = commandParts[0];
        string strategy = commandParts[1];

        // ��������������
        Debug.Log($"�������� - Ŀ��: {targetName}, �Ʋ�: {strategy}");

        // �ж��Ƿ�ִ�з���
        if (targetName == gameObject.name && strategy == "����")
        {
            ExecuteAmbush();
            Debug.Log($"{gameObject.name} ִ�з�������");
        }
    }
    //void Update()
    //{
    //    if (inputCommand == "����")
    //    {
    //        inputCommand = ""; 
    //        ExecuteAmbush();
    //    }
    //}

    // �ⲿ���ã�ִ�з���ָ��
    public void ExecuteAmbush()
    {
        if (isAmbushing)
        {
            Debug.LogWarning("�������״̬�������ظ�����");
            return;
        }

        currentCover = Cover.FindNearestAvailableCover(transform.position, enemyTeamTag);
        if (currentCover == null)
        {
            Debug.LogWarning("δ�ҵ���������");
            return;
        }
        StartCoroutine(AmbushRoutine());
    }

    // ��������Э�̣����ֲ��䣩
    private IEnumerator AmbushRoutine()
    {
        Debug.Log("��ʼ�������ƶ�");
        moveScript.controlMode = 0;
        moveScript.targetPosition = currentCover.transform.position;
        //moveScript.HandleInput();
        //moveScript.FollowPath();

        //moveScript.StartPathfinding(currentCover.transform.position);

        while (Vector2.Distance(transform.position, currentCover.transform.position) > currentCover.occupyRadius)
        {
            yield return null;
        }

        if (currentCover.TryOccupyCover())
        {
            Debug.Log("�ɹ��������״̬");
            moveScript.controlMode = 1;
            EnterAmbushState();
            yield return new WaitUntil(() => Vector2.Distance(transform.position, currentCover.transform.position) > currentCover.occupyRadius);
            //yield return new WaitUntil(() => !isAmbushing);
            currentCover.ReleaseCover();
            ExitAmbushState();
            Debug.Log("�ɹ��˳����״̬");
        }
        else
        {
            Debug.Log("�������ʧ�ܣ�Cover �����򲻿���");
        }
    }

    // �������״̬�����ֲ��䣩
    private void EnterAmbushState()
    {
        isAmbushing = true;
        componentStates.Clear();
        foreach (var component in componentsToDisable)
        {
            if (component != null)
            {
                componentStates[component] = component.enabled;
                component.enabled = false;
            }
        }
        gameObject.layer = hiddenLayer;
        GetComponent<Collider2D>().enabled = false;
    }



    // �˳����״̬�����ֲ��䣩
    public void ExitAmbushState()
    {
        if (!isAmbushing) return;
        foreach (var component in componentsToDisable)
        {
            if (component != null && componentStates.ContainsKey(component))
                component.enabled = componentStates[component];
        }
        gameObject.layer = originalLayer;
        GetComponent<Collider2D>().enabled = true;
        isAmbushing = false;
    }
}