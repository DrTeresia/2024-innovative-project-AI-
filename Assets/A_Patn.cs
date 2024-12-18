using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pathfinding;
using FoW;
using UnityEditor.PackageManager;
using TMPro;


public class A_Path : MonoBehaviour
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
    public string FriendTag; // ���ѵı�ǩ
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // �������ĵ��˼���
    public float detectionRange_of_attack; // ��ⷶΧ

    public Vector3 targetPosition; // Ŀ��λ�ã��������룩

    public Persona myself;

    public List<Persona> personas = new List<Persona>();

    ChatWithOpenAI gpt = new ChatWithOpenAI("");
    PromptGenerate promptGenerate = new PromptGenerate();

    public int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        mSeeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        myself = new Persona(gameObject.name);
        //enemyLayer = LayerMask.NameToLayer("enemy");           //ͼ��Ϊenemy
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
        //if (Input.GetMouseButtonDown(0))              //�������
        //{
        //    //����������λ��ʱ���Ȱ�λ������
        //    animator.SetBool("IsWalkingLeft", false);
        //    animator.SetBool("IsWalkingUp", false);
        //    animator.SetBool("IsWalkingDown", false);

        //    Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    target.z = 0;
        //    CreatePath(target);

        //}

        // ���Ŀ��λ�ñ����ã�����·��
        if (targetPosition != Vector3.zero)
        {
            //�����λ��ʱ���Ȱ�λ������
            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingUp", false);
            animator.SetBool("IsWalkingDown", false);
            targetPosition.z = 0;
            CreatePath(targetPosition);
            targetPosition = Vector3.zero; // ����Ŀ��λ�ã������ظ��ƶ�
        }
        Move();

    }
    private void Move()
    {
        count++;
        //Collider2D[] enemiesInView = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack).Where(c => c.gameObject.layer == enemyLayer.value).ToArray();
        // ��ȡ�����ڼ�ⷶΧ�ڵĶ���
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack);
        personas = new List<Persona>();
        // ���˳���ǩΪenemyTag�Ķ���
        Collider2D[] enemiesInView = new Collider2D[0]; // ��ʼ��Ϊ������
        foreach (var collider in colliders)
        {
            Persona newPerson = new Persona(collider.gameObject.name);
            personas.Add(newPerson);
            if (!collider.gameObject.CompareTag(FriendTag))
            {
                // �����������ĵ������ӵ�������
                // ע�⣺������Ҫ��չ���飬��Ϊ����Ĵ�С�ǹ̶��ģ�����ֱ������Ԫ��
                // ����ʹ��List<Collider2D>���ռ������ת��Ϊ����
                List<Collider2D> tempEnemiesInView = new List<Collider2D>(enemiesInView);
                tempEnemiesInView.Add(collider);
                enemiesInView = tempEnemiesInView.ToArray();
            }
        }

        if(count>=60){
            count=0;
            myself.changeSurroundings(personas);
            string prompt_input = promptGenerate.ReadTextFile(".\\战役背景\\荆州之战.txt")+promptGenerate.create_all_persona_pos_prompt(myself)+promptGenerate.create_persona_choice_prompt();
            Debug.Log(prompt_input);
            gpt.setPrompt(prompt_input);
            gpt.choice();
        }


        if (mPathPointList == null || mPathPointList.Count <= 0)
        {
            // ���û���ƶ�����Ĭ�ϵ�Idle״̬
            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingUp", false);
            animator.SetBool("IsWalkingDown", false);
            bool isIdle = IsIdle();    // �����ɫ��ֹ�������Χ�ĵ���

            //if (isIdle)         //�Ƿ񹥻�
            //{

                if (enemiesInView.Length > 0)
                {
                    rb.velocity = Vector2.zero;
                    animator.SetBool("IsWalkingLeft", false);
                    animator.SetBool("IsWalkingUp", false);
                    animator.SetBool("IsWalkingDown", false);
                // ������⵽�ĵ���
                HandleEnemies(enemiesInView);
                }

            //}
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
                        // ������⵽�ĵ���
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
            //��ɫ���������壬�����岻��Ӱ��
            GameObject name = transform.Find("name").gameObject;
            name.transform.localScale = new Vector3(-Mathf.Abs(name.transform.localScale.x), name.transform.localScale.y, name.transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            GameObject name = transform.Find("name").gameObject;
            name.transform.localScale = new Vector3(Mathf.Abs(name.transform.localScale.x), name.transform.localScale.y, name.transform.localScale.z);
        }
    }
    public bool IsIdle()                                  //�жϾ�ֹ
    {
        //float movementThreshold = 0f; //�ٶ��ж���ֵ
        // ͨ��Rigidbody2D��velocity�������жϽ�ɫ�Ƿ�ֹ
        return rb.velocity.sqrMagnitude <= 0;
    }
    void HandleEnemies(Collider2D[] enemies)               //��Χ���޵���+������ʽ
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
}