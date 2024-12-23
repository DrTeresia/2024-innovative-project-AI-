using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pathfinding;
using FoW;
//using UnityEditor.PackageManager;
using TMPro;

public class move_Auto : MonoBehaviour
{
    public bool isSelected;
    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }
    private Seeker mSeeker;
    private List<Vector3> mPathPointList;  //Ŀ���λ
    private int mCurrentIndex = 0;
    public float speed;
    private Animator animator;// Animator�������
    private Rigidbody2D rb; // �����ƶ���Rigidbody2D���
    //public LayerMask enemyLayer; // �������ڵ�LayerMask
    public string enemyTag; // ���˵ı�ǩ
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // �������ĵ��˼���
    public float detectionRange_of_attack; // ��ⷶΧ
    public float wanderTimer = 1f; // ����ı䷽���ʱ����
    private Vector3 moveDirection = Vector2.left; // ��ʼ�ƶ�����
    private Vector2 position; // С�˵�ǰ��λ��
    // Start is called before the first frame update
    void Start()
    {
        mSeeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //enemyLayer = LayerMask.NameToLayer("enemy");           //ͼ��Ϊenemy
        enemyTag = "Player";                     //��ǩΪenemy
        position = transform.position; // ��ʼ��λ��

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(1))     //�Ҽ�ȡ����ѡ�ı���
        //{
        //    isSelected = false;
        //}

        //����״̬���
        animator.SetBool("IsAttackingUp", false);
        animator.SetBool("IsAttackingDown", false);
        animator.SetBool("IsAttackingLeft", false);

        //if (!isSelected)
        //{
        //    return;
        //}
        if (Input.GetMouseButtonDown(0))
        {
            //����������λ��ʱ���Ȱ�λ������
            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingUp", false);
            animator.SetBool("IsWalkingDown", false);

            Vector2 target = Camera.main.ScreenToWorldPoint(WanderAround());
            //target.z = 0;
            CreatePath(target);

        }
        Move();

    }
    private void Move()
    {
        //Collider2D[] enemiesInView = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack).Where(c => c.gameObject.layer == enemyLayer.value).ToArray();
        // ��ȡ�����ڼ�ⷶΧ�ڵĶ���
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack);

        // ���˳���ǩΪenemyTag�Ķ���
        Collider2D[] enemiesInView = new Collider2D[0]; // ��ʼ��Ϊ������
        foreach (var collider in colliders)
        {
            if (!collider.gameObject.CompareTag(enemyTag))
            {
                // �����������ĵ�����ӵ�������
                // ע�⣺������Ҫ��չ���飬��Ϊ����Ĵ�С�ǹ̶��ģ�����ֱ�����Ԫ��
                // ����ʹ��List<Collider2D>���ռ������ת��Ϊ����
                List<Collider2D> tempEnemiesInView = new List<Collider2D>(enemiesInView);
                tempEnemiesInView.Add(collider);
                enemiesInView = tempEnemiesInView.ToArray();
            }
        }


        if (mPathPointList == null || mPathPointList.Count <= 0)
        {
            // ���û���ƶ�����Ĭ�ϵ�Idle״̬
            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingUp", false);
            animator.SetBool("IsWalkingDown", false);
            bool isIdle = IsIdle();    // �����ɫ��ֹ�������Χ�ĵ���

            if (isIdle)         //�Ƿ񹥻�
            {


                if (enemiesInView.Length > 0)
                {
                    // �����⵽�ĵ���
                    HandleEnemies(enemiesInView);
                }

            }
            return;
        }
        if (Vector2.Distance(transform.position, mPathPointList[mCurrentIndex]) > 0.2f)
        {
            Vector3 dir = (mPathPointList[mCurrentIndex] - transform.position).normalized;  //����
            transform.position += dir * Time.deltaTime * speed;
            //Vector2 dir = (mPathPointList[mCurrentIndex] - transform.position).normalized;
            //float moveDistance = Time.deltaTime * speed;
            //transform.position = Vector2.MoveTowards(transform.position, mPathPointList[mCurrentIndex], moveDistance);

            // �����ƶ���������Animator�����;���
            if (dir.x > 0 && Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            {
                // �����ߣ�ʹ��IsWalkingLeft������ͨ������ʵ��
                animator.SetBool("IsWalkingLeft", true);
                MirrorCharacter(true);
            }
            else if (dir.x < 0 && Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            {
                // �����ߣ�ֱ��ʹ��IsWalkingLeft����
                animator.SetBool("IsWalkingLeft", true);
                MirrorCharacter(false);
            }
            else if (dir.y > 0 && Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
            {
                animator.SetBool("IsWalkingUp", true);
                animator.SetBool("IsWalkingLeft", false);
            }
            else if (dir.y < 0 && Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
            {
                animator.SetBool("IsWalkingDown", true);
                animator.SetBool("IsWalkingLeft", false);
            }
            else
            {
                // ���û���ƶ�����Ĭ�ϵ�Idle״̬
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingUp", false);
                animator.SetBool("IsWalkingDown", false);
            }


        }
        else      //����Ŀ���
        {
            if (mCurrentIndex == mPathPointList.Count - 1)
            {
                // ���û���ƶ�����Ĭ�ϵ�Idle״̬
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingUp", false);
                animator.SetBool("IsWalkingDown", false);

                bool isIdle = IsIdle();    // �����ɫ��ֹ�������Χ�ĵ���

                if (isIdle)         //�Ƿ񹥻�
                {
                    if (enemiesInView.Length > 0)
                    {
                        // �����⵽�ĵ���
                        HandleEnemies(enemiesInView);
                    }

                }
                return;
            }
            mCurrentIndex++;
        }
    }
    private void CreatePath(Vector3 target) //��������ȡ��·��
    {
        mCurrentIndex = 0;
        mSeeker.StartPath(transform.position, target, path =>
        {
            mPathPointList = path.vectorPath;
        });
    }
    void MirrorCharacter(bool mirror)              //������
    {
        // ͨ���ı�scale.xʵ�־���
        if (mirror)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    bool IsIdle()                                  //�жϾ�ֹ
    {
        //float movementThreshold = 0f; //�ٶ��ж���ֵ
        // ͨ��Rigidbody2D��velocity�������жϽ�ɫ�Ƿ�ֹ
        return rb.velocity.sqrMagnitude <= 0;
    }
    void HandleEnemies(Collider2D[] enemies)               //��Χ���޵���+����ʽ
    {
        foreach (var enemy in enemies)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            // ���ݷ������ù�����������
            if (direction.y > 0 && Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) // �������Ϸ�
            {
                animator.SetBool("IsAttackingUp", true);
                animator.SetBool("IsAttackingDown", false);
                animator.SetBool("IsAttackingLeft", false);
            }
            else if (direction.y < 0 && Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) // �������·�
            {
                animator.SetBool("IsAttackingUp", false);
                animator.SetBool("IsAttackingDown", true);
                animator.SetBool("IsAttackingLeft", false);
            }
            else if (direction.x > 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // �������Ҳ�
            {
                animator.SetBool("IsAttackingUp", false);
                animator.SetBool("IsAttackingDown", false);
                animator.SetBool("IsAttackingLeft", true);
                MirrorCharacter(true);
            }

            else if (direction.x < 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // ���������
            {
                animator.SetBool("IsAttackingUp", false);
                animator.SetBool("IsAttackingDown", false);
                animator.SetBool("IsAttackingLeft", true);
                MirrorCharacter(false);
            }
        }
    }
    Vector2 WanderAround()
    {
        position = transform.position + moveDirection * speed * Time.deltaTime;
        // ����wanderTimer����
        wanderTimer -= Time.deltaTime;

        // ��wanderTimerС��0ʱ����ζ����Ҫ�ı䷽����
        if (wanderTimer <= 0f)
        {
            // ����wanderTimer
            wanderTimer = Random.Range(1f, 3f);

            // ���ѡ��һ���µķ���
            float angle = Random.Range(0f, 360f);
            moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }
        return position;

    }

}