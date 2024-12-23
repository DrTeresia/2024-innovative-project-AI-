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
    private List<Vector3> mPathPointList;  //目标点位
    private int mCurrentIndex = 0;
    public float speed;
    private Animator animator;// Animator组件引用
    private Rigidbody2D rb; // 用于移动的Rigidbody2D组件
    //public LayerMask enemyLayer; // 敌人所在的LayerMask
    public string enemyTag; // 敌人的标签
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // 已遇到的敌人集合
    public float detectionRange_of_attack; // 检测范围
    public float wanderTimer = 1f; // 随机改变方向的时间间隔
    private Vector3 moveDirection = Vector2.left; // 初始移动方向
    private Vector2 position; // 小人当前的位置
    // Start is called before the first frame update
    void Start()
    {
        mSeeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //enemyLayer = LayerMask.NameToLayer("enemy");           //图层为enemy
        enemyTag = "Player";                     //标签为enemy
        position = transform.position; // 初始化位置

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(1))     //右键取消框选的兵种
        //{
        //    isSelected = false;
        //}

        //攻击状态清除
        animator.SetBool("IsAttackingUp", false);
        animator.SetBool("IsAttackingDown", false);
        animator.SetBool("IsAttackingLeft", false);

        //if (!isSelected)
        //{
        //    return;
        //}
        if (Input.GetMouseButtonDown(0))
        {
            //点击鼠标获得新位置时，先把位置重置
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
        // 获取所有在检测范围内的对象
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack);

        // 过滤出标签为enemyTag的对象
        Collider2D[] enemiesInView = new Collider2D[0]; // 初始化为空数组
        foreach (var collider in colliders)
        {
            if (!collider.gameObject.CompareTag(enemyTag))
            {
                // 将符合条件的敌人添加到数组中
                // 注意：这里需要扩展数组，因为数组的大小是固定的，不能直接添加元素
                // 可以使用List<Collider2D>来收集，最后转换为数组
                List<Collider2D> tempEnemiesInView = new List<Collider2D>(enemiesInView);
                tempEnemiesInView.Add(collider);
                enemiesInView = tempEnemiesInView.ToArray();
            }
        }


        if (mPathPointList == null || mPathPointList.Count <= 0)
        {
            // 如果没有移动方向，默认的Idle状态
            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingUp", false);
            animator.SetBool("IsWalkingDown", false);
            bool isIdle = IsIdle();    // 如果角色静止，检测周围的敌人

            if (isIdle)         //是否攻击
            {


                if (enemiesInView.Length > 0)
                {
                    // 处理检测到的敌人
                    HandleEnemies(enemiesInView);
                }

            }
            return;
        }
        if (Vector2.Distance(transform.position, mPathPointList[mCurrentIndex]) > 0.2f)
        {
            Vector3 dir = (mPathPointList[mCurrentIndex] - transform.position).normalized;  //方向
            transform.position += dir * Time.deltaTime * speed;
            //Vector2 dir = (mPathPointList[mCurrentIndex] - transform.position).normalized;
            //float moveDistance = Time.deltaTime * speed;
            //transform.position = Vector2.MoveTowards(transform.position, mPathPointList[mCurrentIndex], moveDistance);

            // 根据移动方向设置Animator参数和镜像
            if (dir.x > 0 && Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            {
                // 向右走，使用IsWalkingLeft动画并通过镜像实现
                animator.SetBool("IsWalkingLeft", true);
                MirrorCharacter(true);
            }
            else if (dir.x < 0 && Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            {
                // 向左走，直接使用IsWalkingLeft动画
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
                // 如果没有移动方向，默认的Idle状态
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingUp", false);
                animator.SetBool("IsWalkingDown", false);
            }


        }
        else      //到达目标点
        {
            if (mCurrentIndex == mPathPointList.Count - 1)
            {
                // 如果没有移动方向，默认的Idle状态
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingUp", false);
                animator.SetBool("IsWalkingDown", false);

                bool isIdle = IsIdle();    // 如果角色静止，检测周围的敌人

                if (isIdle)         //是否攻击
                {
                    if (enemiesInView.Length > 0)
                    {
                        // 处理检测到的敌人
                        HandleEnemies(enemiesInView);
                    }

                }
                return;
            }
            mCurrentIndex++;
        }
    }
    private void CreatePath(Vector3 target) //参数：获取的路径
    {
        mCurrentIndex = 0;
        mSeeker.StartPath(transform.position, target, path =>
        {
            mPathPointList = path.vectorPath;
        });
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
    Vector2 WanderAround()
    {
        position = transform.position + moveDirection * speed * Time.deltaTime;
        // 减少wanderTimer计数
        wanderTimer -= Time.deltaTime;

        // 当wanderTimer小于0时，意味着需要改变方向了
        if (wanderTimer <= 0f)
        {
            // 重置wanderTimer
            wanderTimer = Random.Range(1f, 3f);

            // 随机选择一个新的方向
            float angle = Random.Range(0f, 360f);
            moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }
        return position;

    }

}