using UnityEngine;

public class Cover : MonoBehaviour
{
    [Header("基本属性")]
    public int maxCapacity = 3;                  // 最大容纳人数
    public float safeRadius = 2f;                // 有效埋伏半径
    public LayerMask occupantLayer;              // 占用者层级

    [Header("调试显示")]
    public Color availableColor = Color.green;   // 可用状态颜色
    public Color fullColor = Color.red;          // 满载状态颜色

    private int currentOccupants = 0;            // 当前占用人数
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateDebugColor();
    }

    // 检测是否可进入埋伏
    public bool TryOccupyCover()
    {
        if (currentOccupants >= maxCapacity) return false;
        currentOccupants++;
        UpdateDebugColor();
        return true;
    }

    // 离开埋伏时减少占用
    public void ReleaseCover()
    {
        currentOccupants = Mathf.Max(0, currentOccupants - 1);
        UpdateDebugColor();
    }

    // 获取最近的可用Cover（静态方法）
    public static Cover FindNearestAvailableCover(Vector2 origin, string enemyTeamTag)
    {
        Cover[] covers = FindObjectsOfType<Cover>();
        Cover nearestCover = null;
        float minDistance = float.MaxValue;

        foreach (var cover in covers)
        {
            // 检测是否在敌人视野外（简单实现：无遮挡检测）
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