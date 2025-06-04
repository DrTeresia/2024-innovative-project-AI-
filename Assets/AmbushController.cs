using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class AmbushController : MonoBehaviour
{
    [Header("伏击设置")]
    [SerializeField] private string enemyTeamTag = "TeamA";
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();
    [SerializeField] private int originalLayer = 9;         // 记录原始层级
    [SerializeField] private int hiddenLayer = 12;           // 埋伏时切换到的隐藏层级

    [Header("计策")]
    //public string inputCommand; 

    private Move moveScript;
    public Cover currentCover;
    public bool isAmbushing;
    private Dictionary<Behaviour, bool> componentStates = new Dictionary<Behaviour, bool>();
    private bool isStrategyLoaded = false; // 新增加载状态标识

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

    private void OnInputCommandChanged(object command)        //关羽：伏击
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

        //Debug.Log($"解析命令 - 目标: {targetName}, 策略: {strategy}");

        if (targetName == gameObject.name && strategy == "伏击" && moveScript.controlMode == 0)
        {
            Debug.Log($"{gameObject.name} 执行伏击指令");
            ExecuteAmbush();
        }
    }

    // 外部调用：执行伏击指令
    public void ExecuteAmbush()
    {
        if (isAmbushing)
        {
            Debug.LogWarning("已在埋伏状态，不可重复触发");
            return;
        }

        currentCover = Cover.FindNearestAvailableCover(transform.position, enemyTeamTag);
        if (currentCover == null)
        {
            Debug.LogWarning("未找到可用掩体");
            return;
        }
        StartCoroutine(AmbushRoutine());
    }

    // 伏击流程协程
    private IEnumerator AmbushRoutine()
    {
        Debug.Log("开始向掩体移动");
        moveScript.targetPosition = currentCover.transform.position;

        while (Vector2.Distance(transform.position, currentCover.transform.position) > currentCover.occupyRadius)
        {
            yield return null;
        }
        if (currentCover.TryOccupyCover())
        {
            Debug.Log("成功进入埋伏状态");
            EnterAmbushState();
            yield return new WaitUntil(() => Vector2.Distance(transform.position, currentCover.transform.position) > currentCover.occupyRadius);
            currentCover.ReleaseCover();
            ExitAmbushState();
            Debug.Log("成功退出埋伏状态");
        }
        else
        {
            Debug.Log("进入埋伏失败：Cover 已满或不可用");
        }
    }

    // 进入埋伏状态
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



    // 退出埋伏状态（保持不变）
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