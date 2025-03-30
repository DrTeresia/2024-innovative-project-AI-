using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Networking;

[RequireComponent(typeof(Move))]
public class AmbushController : MonoBehaviour
{
    [Header("伏击设置")]
    [SerializeField] private string enemyTeamTag = "TeamA";
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();
    [SerializeField] private int originalLayer;         // 记录原始层级
    [SerializeField] private int hiddenLayer;           // 埋伏时切换到的隐藏层级

    [Header("计策")]
    public string inputCommand; 

    private Move moveScript;
    private Cover currentCover;
    public bool isAmbushing;
    private Dictionary<Behaviour, bool> componentStates = new Dictionary<Behaviour, bool>();
    private bool isStrategyLoaded = false; // 新增加载状态标识

    void Awake()
    {
        moveScript = GetComponent<Move>();
        originalLayer = gameObject.layer;
    }
    void Start()
    {
        StartCoroutine(LoadStrategyFromStreamingAssets());
    }
    void Update()
    {
        if (inputCommand == "伏击")
        {
            inputCommand = ""; 
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

    // 伏击流程协程（保持不变）
    private IEnumerator AmbushRoutine()
    {
        Debug.Log("开始向掩体移动");
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
            Debug.Log("成功进入埋伏状态");
            moveScript.controlMode = 1;
            EnterAmbushState();
            yield return new WaitUntil(() => !isAmbushing);
            currentCover.ReleaseCover();
        }
        else
        {
            Debug.Log("进入埋伏失败：Cover 已满或不可用");
        }
    }

    // 进入埋伏状态（保持不变）
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

    // 新增：异步加载策略配置
    private IEnumerator LoadStrategyFromStreamingAssets()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "predicted_strategy.json");

        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonContent = request.downloadHandler.text;
                    PredictedStrategy strategy = JsonConvert.DeserializeObject<PredictedStrategy>(jsonContent);
                    inputCommand = strategy.strategyName;
                    Debug.Log($"策略加载成功：{inputCommand}");
                    isStrategyLoaded = true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON解析失败: {e.Message}");
                    isStrategyLoaded = false;
                }
            }
            else
            {
                Debug.LogError($"文件加载失败：{request.error}");
                isStrategyLoaded = false;
            }
        }
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