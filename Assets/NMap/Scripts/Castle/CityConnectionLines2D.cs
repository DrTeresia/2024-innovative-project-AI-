using UnityEngine;
using System.Collections.Generic;

public class CityConnectionLines2D : MonoBehaviour
{
    [Header("��������")]
    public float connectionRange = 2f;
    public LayerMask generalLayer;
    public float lineWidth = 0.01f;

    [Header("��ɫ����")]
    public Color sameTagColor = Color.green;
    public Color differentTagColor = Color.red;

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    void Start()
    {
        connectionRange = 10.0f;
        lineWidth = 0.1f; // �����߿�����Ӧ2D����
    }
    void Update()
    {
        UpdateConnections();
    }

    void UpdateConnections()
    {
        ClearAllLines();

        // ʹ��Physics2D����2D���
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, connectionRange, generalLayer);

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject == gameObject) continue;

            CreateConnectionLine(collider.gameObject);
        }
    }

    void CreateConnectionLine(GameObject target)
    {
        GameObject lineObj = new GameObject("ConnectionLine");
        lineObj.transform.SetParent(transform);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        lineRenderers.Add(line);

        // 2D��Ϸͨ��ʹ��XYƽ��
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.transform.position);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        // ʹ��2D�����ȷ�����߿ɼ�
        line.sortingLayerName = "Foreground"; // ����Ϊ���2D�����
        line.sortingOrder = 1;

        Color lineColor = target.CompareTag(gameObject.tag) ? sameTagColor : differentTagColor;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;
    }

    void ClearAllLines()
    {
        foreach (var line in lineRenderers)
        {
            if (line != null && line.gameObject != null)
            {
                Destroy(line.gameObject);
            }
        }
        lineRenderers.Clear();
    }

    // 2D�����л���Բ��Gizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, connectionRange);
    }
}