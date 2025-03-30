using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Networking;

[RequireComponent(typeof(Move))]
public class AmbushController : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private string enemyTeamTag = "TeamA";
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();
    [SerializeField] private int originalLayer;         // ��¼ԭʼ�㼶
    [SerializeField] private int hiddenLayer;           // ���ʱ�л��������ز㼶

    [Header("�Ʋ�")]
    public string inputCommand; 

    private Move moveScript;
    private Cover currentCover;
    public bool isAmbushing;
    private Dictionary<Behaviour, bool> componentStates = new Dictionary<Behaviour, bool>();
    private bool isStrategyLoaded = false; // ��������״̬��ʶ

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
        if (inputCommand == "����")
        {
            inputCommand = ""; 
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
            yield return new WaitUntil(() => !isAmbushing);
            currentCover.ReleaseCover();
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

    // �������첽���ز�������
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
                    Debug.Log($"���Լ��سɹ���{inputCommand}");
                    isStrategyLoaded = true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON����ʧ��: {e.Message}");
                    isStrategyLoaded = false;
                }
            }
            else
            {
                Debug.LogError($"�ļ�����ʧ�ܣ�{request.error}");
                isStrategyLoaded = false;
            }
        }
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