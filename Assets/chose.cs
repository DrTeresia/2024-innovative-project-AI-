using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    public Camera mainCamera;
    private Vector2 startDragPosition; // ��ק��ʼλ��
    private Vector2 endDragPosition; // ��ק����λ��

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
        // ѡ�������ĵ�
        Vector2 centerPosition = (startDragPosition + endDragPosition) / 2;
        // ѡ���Ĵ�С
        Vector2 size = new Vector2(Mathf.Abs(endDragPosition.x - startDragPosition.x), Mathf.Abs(endDragPosition.y - startDragPosition.y));
        // ѡ���ĽǶ�
        float angle = 0f;
        
        LayerMask layerMask = Physics2D.DefaultRaycastLayers;

        Collider2D[] soldiers = Physics2D.OverlapBoxAll(centerPosition, size, angle, layerMask);

        // ����������ײ��ʿ��
        foreach (var soldier in soldiers)
        {
            // ������Ļ�ű�
            Movement a = soldier.GetComponent<Movement>();
            if (a != null)
            {
                //a.SetSelected(true);
            }
        }
    }
}