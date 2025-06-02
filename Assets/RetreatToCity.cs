using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class RetreatController : MonoBehaviour
{
    [Header("城池设置")]
    [SerializeField] private LayerMask cityLayerMask;    // 在Inspector中设置为City图层
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

        string[] commandParts = inputCommand.Split(new[] { '：', ':' }, 2, StringSplitOptions.RemoveEmptyEntries); // 支持中文/英文冒号
        if (commandParts.Length != 2)
        {
            Debug.LogWarning($"指令格式错误: {inputCommand} (正确格式: 名字：命令)");
            return;
        }

        string targetName = commandParts[0].Trim();
        string strategy = commandParts[1].Trim();

        Debug.Log($"解析命令 - 目标: {targetName}, 策略: {strategy}");

        if (targetName == gameObject.name && strategy == "撤退")
        {
            Debug.Log($"{gameObject.name} 执行撤退指令");
            ExecuteRetreat();
        }
        else if (targetName == gameObject.name && strategy == "死守")
        {
            Debug.Log($"{gameObject.name} 执行死守指令");
            ExecuteRetreat();
        }
    }

    public void ExecuteRetreat()
    {
        if (isInCity)
        {
            Debug.LogWarning("已在城池中");
            return;
        }

        GameObject targetCity = FindNearestCastleWithSameTag();
        if (targetCity == null)
        {
            Debug.LogWarning("未找到符合要求的城池");
            return;
        }
        StartCoroutine(RetreatRoutine(targetCity.transform));
    }

    // 按标签查找最近的城池
    private GameObject FindNearestCastleWithSameTag()
    {
        GameObject[] allCastles = GameObject.FindGameObjectsWithTag(gameObject.tag);
        GameObject nearestCastle = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject castle in allCastles)
        {
            // 检查城池图层
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

        Debug.Log("成功抵达城池");
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