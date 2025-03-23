using UnityEngine;

public class Cover : MonoBehaviour
{
    [Header("��������")]
    public int maxCapacity = 3;                  // �����������
    public float safeRadius = 2f;                // ��Ч����뾶
    public LayerMask occupantLayer;              // ռ���߲㼶

    [Header("������ʾ")]
    public Color availableColor = Color.green;   // ����״̬��ɫ
    public Color fullColor = Color.red;          // ����״̬��ɫ

    private int currentOccupants = 0;            // ��ǰռ������
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateDebugColor();
    }

    // ����Ƿ�ɽ������
    public bool TryOccupyCover()
    {
        if (currentOccupants >= maxCapacity) return false;
        currentOccupants++;
        UpdateDebugColor();
        return true;
    }

    // �뿪���ʱ����ռ��
    public void ReleaseCover()
    {
        currentOccupants = Mathf.Max(0, currentOccupants - 1);
        UpdateDebugColor();
    }

    // ��ȡ����Ŀ���Cover����̬������
    public static Cover FindNearestAvailableCover(Vector2 origin, string enemyTeamTag)
    {
        Cover[] covers = FindObjectsOfType<Cover>();
        Cover nearestCover = null;
        float minDistance = float.MaxValue;

        foreach (var cover in covers)
        {
            // ����Ƿ��ڵ�����Ұ�⣨��ʵ�֣����ڵ���⣩
            Collider2D enemyInSight = Physics2D.OverlapCircle(cover.transform.position, cover.safeRadius, LayerMask.GetMask(enemyTeamTag));
            if (enemyInSight != null) continue;

            float distance = Vector2.Distance(origin, cover.transform.position);
            if (distance < minDistance && cover.currentOccupants < cover.maxCapacity)
            {
                minDistance = distance;
                nearestCover = cover;
            }
        }
        return nearestCover;
    }

    void UpdateDebugColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = (currentOccupants < maxCapacity) ? availableColor : fullColor;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, safeRadius);
    }
}