using UnityEngine;
using System.Collections.Generic;

public class CityConnectionLines : MonoBehaviour
{
    [Header("连线设置")]
    public float connectionRange = 10f;
    public LayerMask generalLayer;
    public float lineWidth = 0.5f; // 临时加大线宽方便调试

    [Header("颜色设置")]
    public Color sameTagColor = Color.green;
    public Color differentTagColor = Color.red;

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    void Update()
    {
        UpdateConnections();
    }

    void UpdateConnections()
    {
        Debug.Log("=== 开始更新连线 ===");

        ClearAllLines();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, connectionRange);
        Debug.Log($"在{connectionRange}范围内检测到{hitColliders.Length}个对象");

        foreach (var collider in hitColliders)
        {
            //if (collider.gameObject == gameObject)
            //{
            //    Debug.Log("跳过自身: " + gameObject.name);
            //    continue;
            //}

            Debug.Log("正在连接到: " + collider.gameObject.name);
            CreateConnectionLine(collider.gameObject);
        }
    }

    void CreateConnectionLine(GameObject target)
    {
        Debug.Log($"创建从{gameObject.name}(tag:{gameObject.tag})到{target.name}(tag:{target.tag})的连线");

        GameObject lineObj = new GameObject("ConnectionLine");
        lineObj.transform.SetParent(transform);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        lineRenderers.Add(line);

        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.transform.position);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        bool isSameTag = target.CompareTag(gameObject.tag);
        Debug.Log($"Tag比较结果: {(isSameTag ? "相同" : "不同")}");

        Color lineColor = isSameTag ? sameTagColor : differentTagColor;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;

        // 确保LineRenderer激活
        line.enabled = true;
        Debug.Log($"连线已创建，颜色: {lineColor}");
    }

    void ClearAllLines()
    {
        Debug.Log($"清除{lineRenderers.Count}条现有连线");
        foreach (var line in lineRenderers)
        {
            if (line != null && line.gameObject != null)
            {
                Destroy(line.gameObject);
            }
        }
        lineRenderers.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, connectionRange);
    }
}