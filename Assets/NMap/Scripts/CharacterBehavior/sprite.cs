using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class sprite : MonoBehaviour
{
    //float moveSpeed = 0.05f; // ��ɫ�ƶ��ٶ�
    //private float movementThreshold = 0.1f; //�ٶ��ж���ֵ
    private Vector2 targetPosition; // Ŀ��λ��
    private Animator animator;// Animator�������
    private Rigidbody2D rb; // �����ƶ���Rigidbody2D���
    public LayerMask enemyLayer; // �������ڵ�LayerMask
    public GameObject dialogPanel; // �Ի����GameObject
    private bool isDialogActive = false; // �Ի����Ƿ񼤻�
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // �������ĵ��˼���

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        dialogPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {   //����״̬���
        animator.SetBool("IsAttackingUp", false);
        animator.SetBool("IsAttackingDown", false);
        animator.SetBool("IsAttackingLeft", false);


        float detectionRange_of_attack = 0.2f; // ��ⷶΧ

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = mousePos;
        }
        

        //�����б�
        Collider2D[] enemiesInView = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack).Where(c => c.gameObject.layer == enemyLayer.value).ToArray();

        // �ƶ���ɫ��Ŀ��λ��
        if (targetPosition != Vector2.zero)
        {
            float moveSpeed = 10.0f; // ��ɫ�ƶ��ٶ�
            Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
            //rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);

            float moveDistance = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveDistance);

            // �����ƶ���������Animator�����;���
            if (moveDirection.x > 0 && Mathf.Abs(moveDirection.x) >= Mathf.Abs(moveDirection.y))
            {
                // �����ߣ�ʹ��IsWalkingLeft������ͨ������ʵ��
                animator.SetBool("IsWalkingLeft", true);
                MirrorCharacter(true);
            }
            else if (moveDirection.x < 0 && Mathf.Abs(moveDirection.x) >= Mathf.Abs(moveDirection.y))
            {
                // �����ߣ�ֱ��ʹ��IsWalkingLeft����
                animator.SetBool("IsWalkingLeft", true);
                MirrorCharacter(false);
            }
            else if (moveDirection.y > 0 && Mathf.Abs(moveDirection.x) < Mathf.Abs(moveDirection.y))
            {
                animator.SetBool("IsWalkingUp", true);
                animator.SetBool("IsWalkingLeft", false);
            }
            else if (moveDirection.y < 0 && Mathf.Abs(moveDirection.x) < Mathf.Abs(moveDirection.y))
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
        bool isIdle = IsIdle();    // �����ɫ��ֹ�������Χ�ĵ���

        if (isIdle)         //�Ƿ񹥻�
        {
            //Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange);
            //foreach (var enemy in enemies)
            //{
            //    if (enemy.CompareTag("Enemy"))
            //    {
            //        // ���ݵ��˵ķ�����й���
            //        Vector2 direction = (enemy.transform.position - transform.position).normalized;
            //        AttackInDirection(direction);
            //    }
            //}

            
            
            if (enemiesInView.Length > 0)
            {
                // �����⵽�ĵ���
                HandleEnemies(enemiesInView);
            }

        }

        float triggerDistance = 0.2f; // �����Ի��ľ���

        if (isIdle)
        {
            if(enemiesInView.Length > 0)
            {
                Dialogtrigger(enemiesInView, encounteredEnemies);
            }
        }

        //foreach (var enemy in enemiesInView)
        //{
        //    if (enemy.gameObject.layer == enemyLayer.value && !isDialogActive)
        //    {
        //        // ��ʾ�Ի���
        //        OpenDialog();
        //        isDialogActive = true;
        //        break; // ����һ��ֻ����һ�����˶Ի�
        //    }
        //}

        //// ���ո���Ƿ񱻰����ԹرնԻ���
        //if (isDialogActive && Input.GetKeyDown(KeyCode.Space))
        //{
        //    CloseDialog();
        //}

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

    void Dialogtrigger(Collider2D[] enemies, HashSet<GameObject> encounteredEnemies)
    {
        // �����Χ�ĵ���
        //Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.layer == enemyLayer.value && !encounteredEnemies.Contains(enemy.gameObject))
            {
                // ���˵�һ�ν��뷶Χ����ʾ�Ի���
                OpenDialog();
                isDialogActive = true;
                encounteredEnemies.Add(enemy.gameObject); // ��������ӵ��������ļ�����
                break; // ����һ��ֻ����һ�����˶Ի�
            }
        }

        // ����Ի����Ѽ�����ո���Ƿ񱻰����ԹرնԻ���
        if (isDialogActive && Input.GetKeyDown(KeyCode.Space))
        {
            CloseDialog();
        }
    }

    void OpenDialog()
    {
        // ��ʾ�Ի���
        dialogPanel.SetActive(true);
    }

     void CloseDialog()
    {
        // ���ضԻ���
        dialogPanel.SetActive(false);
        isDialogActive = false;
    }
}
