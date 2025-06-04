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
    [SerializeField] private int originalLayer = 9;         // ��¼ԭʼ�㼶
    [SerializeField] private int hiddenLayer = 12;           // ���ʱ�л��������ز㼶

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

    private void OnInputCommandChanged(object command)        //���𣺷���
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

        //Debug.Log($"�������� - Ŀ��: {targetName}, ����: {strategy}");

        if (targetName == gameObject.name && strategy == "����" && moveScript.controlMode == 0)
        {
            Debug.Log($"{gameObject.name} ִ�з���ָ��");
            ExecuteAmbush();
        }
    }

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

    // ��������Э��
    private IEnumerator AmbushRoutine()
    {
        Debug.Log("��ʼ�������ƶ�");
        moveScript.targetPosition = currentCover.transform.position;

        while (Vector2.Distance(transform.position, currentCover.transform.position) > currentCover.occupyRadius)
        {
            yield return null;
        }
        if (currentCover.TryOccupyCover())
        {
            Debug.Log("�ɹ��������״̬");
            EnterAmbushState();
            yield return new WaitUntil(() => Vector2.Distance(transform.position, currentCover.transform.position) > currentCover.occupyRadius);
            currentCover.ReleaseCover();
            ExitAmbushState();
            Debug.Log("�ɹ��˳����״̬");
        }
        else
        {
            Debug.Log("�������ʧ�ܣ�Cover �����򲻿���");
        }
    }

    // �������״̬
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