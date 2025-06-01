using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class RetreatController : MonoBehaviour
{
    [Header("�ǳ�����")]
    [SerializeField] private LayerMask cityLayerMask;    // ��Inspector������ΪCityͼ��
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();
    [SerializeField] private int originalLayer;
    [SerializeField] private int hiddenLayer;

    private Move moveScript;
    public bool isInCity;
    private Dictionary<Behaviour, bool> componentStates = new Dictionary<Behaviour, bool>();

    void Awake()
    {
        moveScript = GetComponent<Move>();
        originalLayer = gameObject.layer;
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

        string[] commandParts = inputCommand.Split(new[] { '��', ':' }, 2, StringSplitOptions.RemoveEmptyEntries); // ֧������/Ӣ��ð��
        if (commandParts.Length != 2)
        {
            Debug.LogWarning($"ָ���ʽ����: {inputCommand} (��ȷ��ʽ: ���֣�����)");
            return;
        }

        string targetName = commandParts[0].Trim();
        string strategy = commandParts[1].Trim();

        Debug.Log($"�������� - Ŀ��: {targetName}, ����: {strategy}");

        if (targetName == gameObject.name && strategy == "����")
        {
            Debug.Log($"{gameObject.name} ִ�г���ָ��");
            ExecuteRetreat();
        }
        else if (targetName == gameObject.name && strategy == "����")
        {
            Debug.Log($"{gameObject.name} ִ������ָ��");
            ExecuteRetreat();
        }
    }

    public void ExecuteRetreat()
    {
        if (isInCity)
        {
            Debug.LogWarning("���ڳǳ���");
            return;
        }

        GameObject targetCity = FindNearestCastleWithSameTag();
        if (targetCity == null)
        {
            Debug.LogWarning("δ�ҵ�����Ҫ��ĳǳ�");
            return;
        }
        StartCoroutine(RetreatRoutine(targetCity.transform));
    }

    // ����ǩ��������ĳǳ�
    private GameObject FindNearestCastleWithSameTag()
    {
        GameObject[] allCastles = GameObject.FindGameObjectsWithTag(gameObject.tag);
        GameObject nearestCastle = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject castle in allCastles)
        {
            // ���ǳ�ͼ��
            if (cityLayerMask != (cityLayerMask | (1 << castle.layer)))
                continue;

            float distance = Vector2.Distance(transform.position, castle.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCastle = castle;
            }
        }
        return nearestCastle;
    }

    private IEnumerator RetreatRoutine(Transform cityTransform)
    {
        moveScript.controlMode = 0;
        moveScript.targetPosition = cityTransform.position;

        while (Vector2.Distance(transform.position, cityTransform.position) > 0.5f)
        {
            yield return null;
        }

        Debug.Log("�ɹ��ִ�ǳ�");
        moveScript.controlMode = 1;
        EnterCityState();
    }

    private void EnterCityState()
    {
        isInCity = true;
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

    public void ExitCityState()
    {
        if (!isInCity) return;
        foreach (var component in componentsToDisable)
        {
            if (component != null && componentStates.ContainsKey(component))
                component.enabled = componentStates[component];
        }
        gameObject.layer = originalLayer;
        GetComponent<Collider2D>().enabled = true;
        isInCity = false;
    }
}