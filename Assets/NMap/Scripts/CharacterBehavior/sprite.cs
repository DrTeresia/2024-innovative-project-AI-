using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pathfinding;

public class sprite : MonoBehaviour
{
    //float moveSpeed = 0.05f; // 角色移动速度
    //private float movementThreshold = 0.1f; //速度判断阈值
    private Vector2 targetPosition; // 目标位置
    private Animator animator;// Animator组件引用
    private Rigidbody2D rb; // 用于移动的Rigidbody2D组件
    public LayerMask enemyLayer; // 敌人所在的LayerMask
    //public GameObject dialogPanel; // 对话框的GameObject
    //private bool isDialogActive = false; // 对话框是否激活
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // 已遇到的敌人集合

    public float detectionRange_of_attack = 0.2f; // 检测范围

    public float moveSpeed = 0.5f; // 角色移动速度

    // Start is called before the first frame update


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //dialogPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {   //攻击状态清除
        animator.SetBool("IsAttackingUp", false);
        animator.SetBool("IsAttackingDown", false);
        animator.SetBool("IsAttackingLeft", false);


        //获取鼠标点击位置
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = mousePos;


        }


        // 移动角色到目标位置
        if (targetPosition != Vector2.zero)
        {
            Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
            //rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);

            float moveDistance = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveDistance);

            // 根据移动方向设置Animator参数和镜像
            if (moveDirection.x > 0 && Mathf.Abs(moveDirection.x) >= Mathf.Abs(moveDirection.y))
            {
                // 向右走，使用IsWalkingLeft动画并通过镜像实现
                animator.SetBool("IsWalkingLeft", true);
                MirrorCharacter(true);
            }
            else if (moveDirection.x < 0 && Mathf.Abs(moveDirection.x) >= Mathf.Abs(moveDirection.y))
            {
                // 向左走，直接使用IsWalkingLeft动画
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
                // 如果没有移动方向，默认的Idle状态
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingUp", false);
                animator.SetBool("IsWalkingDown", false);
            }

        }
        bool isIdle = IsIdle();    // 如果角色静止，检测周围的敌人

        if (isIdle)         //是否攻击
        {

            //敌人列表
            Collider2D[] enemiesInView = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack).Where(c => c.gameObject.layer == enemyLayer.value).ToArray();

            if (enemiesInView.Length > 0)
            {
                // 处理检测到的敌人
                HandleEnemies(enemiesInView);
            }

        }


    }
    void MirrorCharacter(bool mirror)              //镜像器
    {
        // 通过改变scale.x实现镜像
        if (mirror)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    bool IsIdle()                                  //判断静止
    {
         //float movementThreshold = 0f; //速度判断阈值
        // 通过Rigidbody2D的velocity属性来判断角色是否静止
        return rb.velocity.sqrMagnitude <= 0;
    }
    void HandleEnemies(Collider2D[] enemies)               //周围有无敌人+处理方式
    {
        foreach (var enemy in enemies)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            // 根据方向设置攻击动画参数
            if (direction.y > 0 && Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) // 敌人在上方
            {
                animator.SetBool("IsAttackingUp", true);
                animator.SetBool("IsAttackingDown", false);
                animator.SetBool("IsAttackingLeft", false);
            }
            else if (direction.y < 0 && Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) // 敌人在下方
            {
                animator.SetBool("IsAttackingUp", false);
                animator.SetBool("IsAttackingDown", true);
                animator.SetBool("IsAttackingLeft", false);
            }
            else if (direction.x > 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // 敌人在右侧
            {
                animator.SetBool("IsAttackingUp", false);
                animator.SetBool("IsAttackingDown", false);
                animator.SetBool("IsAttackingLeft", true);
                MirrorCharacter(true);
            }

            else if (direction.x < 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // 敌人在左侧
            {
                animator.SetBool("IsAttackingUp", false);
                animator.SetBool("IsAttackingDown", false);
                animator.SetBool("IsAttackingLeft", true);
                MirrorCharacter(false);
            }
        }
    }



}
