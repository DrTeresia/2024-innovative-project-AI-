using UnityEngine;
using System.Collections.Generic;

public class CityConnectionLines : MonoBehaviour
{
    [Header("��������")]
    public float connectionRange = 10f;
    public LayerMask generalLayer;
    public float lineWidth = 0.5f; // ��ʱ�Ӵ��߿������

    [Header("��ɫ����")]
    public Color sameTagColor = Color.green;
    public Color differentTagColor = Color.red;

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    void Update()
    {
        UpdateConnections();
    }

    void UpdateConnections()
    {
        Debug.Log("=== ��ʼ�������� ===");

        ClearAllLines();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, connectionRange);
        Debug.Log($"��{connectionRange}��Χ�ڼ�⵽{hitColliders.Length}������");

        foreach (var collider in hitColliders)
        {
            //if (collider.gameObject == gameObject)
            //{
            //    Debug.Log("��������: " + gameObject.name);
            //    continue;
            //}

            Debug.Log("�������ӵ�: " + collider.gameObject.name);
            CreateConnectionLine(collider.gameObject);
        }
    }

    void CreateConnectionLine(GameObject target)
    {
        Debug.Log($"������{gameObject.name}(tag:{gameObject.tag})��{target.name}(tag:{target.tag})������");

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
        Debug.Log($"Tag�ȽϽ��: {(isSameTag ? "��ͬ" : "��ͬ")}");

        Color lineColor = isSameTag ? sameTagColor : differentTagColor;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;

        // ȷ��LineRenderer����
        line.enabled = true;
        Debug.Log($"�����Ѵ�������ɫ: {lineColor}");
    }

    void ClearAllLines()
    {
        Debug.Log($"���{lineRenderers.Count}����������");
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