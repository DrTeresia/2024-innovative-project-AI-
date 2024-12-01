using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    public Camera mainCamera;
    private Vector2 startDragPosition; // 拖拽开始位置
    private Vector2 endDragPosition; // 拖拽结束位置

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startDragPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            endDragPosition = startDragPosition;
        }

        if (Input.GetMouseButton(0))
        {
            endDragPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            CheckForSoldiers();
        }
    }

    void CheckForSoldiers()
    {
        // 选择框的中心点
        Vector2 centerPosition = (startDragPosition + endDragPosition) / 2;
        // 选择框的大小
        Vector2 size = new Vector2(Mathf.Abs(endDragPosition.x - startDragPosition.x), Mathf.Abs(endDragPosition.y - startDragPosition.y));
        // 选择框的角度
        float angle = 0f;
        
        LayerMask layerMask = Physics2D.DefaultRaycastLayers;

        Collider2D[] soldiers = Physics2D.OverlapBoxAll(centerPosition, size, angle, layerMask);

        // 遍历所有碰撞的士兵
        foreach (var soldier in soldiers)
        {
            // 找物体的活动脚本
            NewBehaviourScript a = soldier.GetComponent<NewBehaviourScript>();
            if (a != null)
            {
                a.SetSelected(true);
            }
        }
    }
}