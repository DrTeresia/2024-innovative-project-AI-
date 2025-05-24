using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class RetreatController : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private LayerMask cityLayerMask;    // ��Inspector������ΪCity�㼶
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();
    [SerializeField] private int originalLayer;
    [SerializeField] private int hiddenLayer;

    //[Header("ָ��")]
    //public string inputCommand; //  ���˵��ǳ�,city

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
    //void Update()
    //{
    //    if (string.IsNullOrEmpty(inputCommand))
    //    {
    //        return;
    //    }

    //    // ����ָ���ʽ�����˵��ǳ�,�ǳ�����
    //    string[] commandParts = inputCommand.Split(',');
    //    //inputCommand = "";

    //    if (commandParts[0].Trim() != "���˵��ǳ�")
    //    {
    //        return;
    //    }

    //    string targetCityName = commandParts[1].Trim();
    //    ExecuteRetreat(targetCityName);
    //}
    private void OnInputCommandChanged(object command)
    {
        string inputCommand = (string)command;
        string[] parts = inputCommand.Split(',');
        if (parts.Length >= 2 && parts[0].Trim() == "���˵��ǳ�")
        {
            ExecuteRetreat(parts[1].Trim());
        }else if(inputCommand == "����")
        {

        }
    }
    public void ExecuteRetreat(string cityName)
    {
        if (isInCity)
        {
            Debug.LogWarning("���ڳǳ���");
            return;
        }

        GameObject targetCity = FindCityByName(cityName);
        if (targetCity == null)
        {
            Debug.LogWarning($"δ�ҵ�Ŀ��ǳأ�{cityName}");
            return;
        }
        StartCoroutine(RetreatRoutine(targetCity.transform));
    }

    // �������ƺͲ㼶���ҳǳ�
    private GameObject FindCityByName(string targetName)
    {
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.layer == (int)Mathf.Log(cityLayerMask.value, 2) &&
                obj.name.Equals(targetName, System.StringComparison.OrdinalIgnoreCase))
            {
                return obj;
            }
        }
        return null;
    }

    private IEnumerator RetreatRoutine(Transform cityTransform)
    {
        //Debug.Log("��ʼ��ǳ��ƶ�");
        moveScript.controlMode = 0;
        //moveScript.StartPathfinding(cityTransform.position);
        moveScript.targetPosition = cityTransform.position;

        while (Vector2.Distance(transform.position, cityTransform.position) > 0.5f)
        {
            yield return null;
        }

        Debug.Log("�ɹ�����ǳ�");
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