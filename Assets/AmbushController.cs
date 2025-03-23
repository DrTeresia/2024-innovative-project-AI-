using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class AmbushController : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private string enemyTeamTag = "TeamA";  // �з������ǩ
    [SerializeField] private LayerMask componentMask;        // ��Ҫ���õ�������ڲ㼶

    private Move moveScript;
    private Cover currentCover;
    private bool isAmbushing;

    void Awake()
    {
        moveScript = GetComponent<Move>();
    }

    // �ⲿ���ã�ִ�з���ָ��
    public void ExecuteAmbush()
    {
        if (isAmbushing) return;

        // Ѱ������Ŀ���Cover
        currentCover = Cover.FindNearestAvailableCover(transform.position, enemyTeamTag);
        if (currentCover == null) return;

        // �����ƶ�Э��
        StartCoroutine(AmbushRoutine());
    }

    // ��������Э��
    private IEnumerator AmbushRoutine()
    {
        // �ƶ���Coverλ��
        moveScript.StartPathfinding(currentCover.transform.position);

        // �ȴ�����Ŀ��
        while (moveScript.IsPathfinding)
            yield return null;

        // ���Խ������״̬
        if (currentCover.TryOccupyCover())
        {
            EnterAmbushState();
            yield return new WaitUntil(() => !isAmbushing); // �ȴ�ȡ�����
            currentCover.ReleaseCover();
        }
    }

    // �������״̬
    private void EnterAmbushState()
    {
        isAmbushing = true;

        // ����ָ��������繥���������ȣ�
        foreach (var component in GetComponents<Behaviour>())
        {
            if ((componentMask & (1 << component.GetType().GetHashCode())) != 0)
                component.enabled = false;
        }

        // ���ؽ�ɫ
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false; // ��ѡ��������ײ
    }

    // �˳����״̬
    public void ExitAmbushState()
    {
        if (!isAmbushing) return;

        // �������
        foreach (var component in GetComponents<Behaviour>())
        {
            if ((componentMask & (1 << component.GetType().GetHashCode())) != 0)
                component.enabled = true;
        }

        // ��ʾ��ɫ
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;

        isAmbushing = false;
    }
}