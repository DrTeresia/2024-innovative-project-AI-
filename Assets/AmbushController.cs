using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class AmbushController : MonoBehaviour
{
    [Header("伏击设置")]
    [SerializeField] private string enemyTeamTag = "TeamA";  // 敌方队伍标签
    [SerializeField] private LayerMask componentMask;        // 需要禁用的组件所在层级

    private Move moveScript;
    private Cover currentCover;
    private bool isAmbushing;

    void Awake()
    {
        moveScript = GetComponent<Move>();
    }

    // 外部调用：执行伏击指令
    public void ExecuteAmbush()
    {
        if (isAmbushing) return;

        // 寻找最近的可用Cover
        currentCover = Cover.FindNearestAvailableCover(transform.position, enemyTeamTag);
        if (currentCover == null) return;

        // 启动移动协程
        StartCoroutine(AmbushRoutine());
    }

    // 伏击流程协程
    private IEnumerator AmbushRoutine()
    {
        // 移动到Cover位置
        moveScript.StartPathfinding(currentCover.transform.position);

        // 等待到达目标
        while (moveScript.IsPathfinding)
            yield return null;

        // 尝试进入埋伏状态
        if (currentCover.TryOccupyCover())
        {
            EnterAmbushState();
            yield return new WaitUntil(() => !isAmbushing); // 等待取消埋伏
            currentCover.ReleaseCover();
        }
    }

    // 进入埋伏状态
    private void EnterAmbushState()
    {
        isAmbushing = true;

        // 禁用指定组件（如攻击、动画等）
        foreach (var component in GetComponents<Behaviour>())
        {
            if ((componentMask & (1 << component.GetType().GetHashCode())) != 0)
                component.enabled = false;
        }

        // 隐藏角色
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false; // 可选：禁用碰撞
    }

    // 退出埋伏状态
    public void ExitAmbushState()
    {
        if (!isAmbushing) return;

        // 启用组件
        foreach (var component in GetComponents<Behaviour>())
        {
            if ((componentMask & (1 << component.GetType().GetHashCode())) != 0)
                component.enabled = true;
        }

        // 显示角色
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;

        isAmbushing = false;
    }
}