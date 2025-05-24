using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class RetreatController : MonoBehaviour
{
    [Header("撤退设置")]
    [SerializeField] private LayerMask cityLayerMask;    // 在Inspector中设置为City层级
    [SerializeField] private List<Behaviour> componentsToDisable = new List<Behaviour>();
    [SerializeField] private int originalLayer;
    [SerializeField] private int hiddenLayer;

    //[Header("指令")]
    //public string inputCommand; //  撤退到城池,city

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

    //    // 解析指令格式：撤退到城池,城池名称
    //    string[] commandParts = inputCommand.Split(',');
    //    //inputCommand = "";

    //    if (commandParts[0].Trim() != "撤退到城池")
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
        if (parts.Length >= 2 && parts[0].Trim() == "撤退到城池")
        {
            ExecuteRetreat(parts[1].Trim());
        }else if(inputCommand == "死守")
        {

        }
    }
    public void ExecuteRetreat(string cityName)
    {
        if (isInCity)
        {
            Debug.LogWarning("已在城池内");
            return;
        }

        GameObject targetCity = FindCityByName(cityName);
        if (targetCity == null)
        {
            Debug.LogWarning($"未找到目标城池：{cityName}");
            return;
        }
        StartCoroutine(RetreatRoutine(targetCity.transform));
    }

    // 根据名称和层级查找城池
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
        //Debug.Log("开始向城池移动");
        moveScript.controlMode = 0;
        //moveScript.StartPathfinding(cityTransform.position);
        moveScript.targetPosition = cityTransform.position;

        while (Vector2.Distance(transform.position, cityTransform.position) > 0.5f)
        {
            yield return null;
        }

        Debug.Log("成功进入城池");
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