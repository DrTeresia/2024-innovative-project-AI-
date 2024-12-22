<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    public GameObject skillPrefab; // ����Ԥ����
    public float skillRange;// ���ܾ����ɫ�ľ���
    //private Animator animator;// Animator�������


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // ���WASD���İ����¼�
        if (Input.GetKeyDown(KeyCode.W))
        {
            ReleaseSkill(new Vector2(0, 1)); // �����ͷż���
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ReleaseSkill(new Vector2(-1, 0)); // �����ͷż���
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ReleaseSkill(new Vector2(0, -1)); // �����ͷż���
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ReleaseSkill(new Vector2(1, 0)); // �����ͷż���
        }
    }

    void ReleaseSkill(Vector2 direction)
    {
        if (skillPrefab != null)
        {
            // ������������׼����ȷ�������ͷŵľ���һ��
            Vector2 normalizedDirection = direction.normalized;

            // ���㼼�ܵ�ʵ����λ�ã���ɫλ�� + ���� * �����ͷž���
            Vector3 skillPosition = transform.position + new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * skillRange;

            // ʵ��������Ԥ����
            Instantiate(skillPrefab, skillPosition, Quaternion.identity);
        }
    }
}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    public GameObject skillPrefab; // ����Ԥ����
    public float skillRange;// ���ܾ����ɫ�ľ���
    //private Animator animator;// Animator�������


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // ���WASD���İ����¼�
        if (Input.GetKeyDown(KeyCode.W))
        {
            ReleaseSkill(new Vector2(0, 1)); // �����ͷż���
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ReleaseSkill(new Vector2(-1, 0)); // �����ͷż���
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ReleaseSkill(new Vector2(0, -1)); // �����ͷż���
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ReleaseSkill(new Vector2(1, 0)); // �����ͷż���
        }
    }

    void ReleaseSkill(Vector2 direction)
    {
        if (skillPrefab != null)
        {
            // ������������׼����ȷ�������ͷŵľ���һ��
            Vector2 normalizedDirection = direction.normalized;

            // ���㼼�ܵ�ʵ����λ�ã���ɫλ�� + ���� * �����ͷž���
            Vector3 skillPosition = transform.position + new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * skillRange;

            // ʵ��������Ԥ����
            Instantiate(skillPrefab, skillPosition, Quaternion.identity);
        }
    }
}
>>>>>>> origin/main
