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



public class Movement : MonoBehaviour
{
    //public bool isSelected;
    //public void SetSelected(bool selected)
    //{
    //    isSelected = selected;
    //}
    private Seeker mSeeker;
    private List<Vector3> mPathPointList;  //目标点位
    private int mCurrentIndex = 0;
    public float speed;
    private Animator animator;// Animator组件引用
    private Rigidbody2D rb; // 用于移动的Rigidbody2D组件
    //public LayerMask enemyLayer; // 敌人所在的LayerMask
    public string enemyTag1; // 敌人的标签
    public string enemyTag2;
    public string enemyTag3;
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // 已遇到的敌人集合
    public float detectionRange_of_attack; // 检测范围

    public Vector3 targetPosition; // 目标位置（坐标输入）

    private SPUM_Prefabs spumPrefabs; // 控制动画的脚本引用

    private GameObject name;
    private string nameString;
    private GameObject message;

    //gpt相关
    string responseString;
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
        //enemyLayer = LayerMask.NameToLayer("enemy");           //图层为enemy
        //enemyTag = "enemy";                     //标签为enemy
        // 获取SPUM_Prefabs脚本的引用
        spumPrefabs = GetComponent<SPUM_Prefabs>();
        //gpt相关
        myself = new Persona(gameObject.name);
        Debug.Log("myself: " + myself.name);

        //找到子类name
        name = transform.Find("name").gameObject;
        nameString = name.GetComponent<TextMesh>().text;

        message = transform.Find("message").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(1))     //右键取消框选的兵种
        //{
        //    isSelected = false;
        //}

        ////攻击状态清除
        //animator.SetBool("IsAttackingUp", false);
        //animator.SetBool("IsAttackingDown", false);
        //animator.SetBool("IsAttackingLeft", false);

        //if (!isSelected)
        //{
        //    return;
        //}

        //if (Input.GetMouseButtonDown(0))              //鼠标输入
        //{
        //    ////点击鼠标获得新位置时，先把位置重置
        //    //animator.SetBool("IsWalkingLeft", false);
        //    //animator.SetBool("IsWalkingUp", false);
        //    //animator.SetBool("IsWalkingDown", false);

        //    Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    target.z = 0;
        //    CreatePath(target);

        //}
        // 如果目标位置被设置，创建路径
        if (targetPosition != Vector3.zero)
        {
            //获得新位置时，先把位置重置
            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingUp", false);
            animator.SetBool("IsWalkingDown", false);
            targetPosition.z = 0;
            CreatePath(targetPosition);
            targetPosition = Vector3.zero; // 重置目标位置，避免重复移动
        }
        count++;
        int randomWaitingTime = Random.Range(300, 600);
        if (count >= randomWaitingTime && !isChoice)
        {
            gptChoice();
            count = 0;
            Debug.Log("todo: " + responseString);
            //语句解析。responseString有多行，行数不固定，每行是一个语句
            //语句结构为：[角色名],[动作],[目标]
            //动作有：撤退，结盟，进攻，佯攻，防守
            //相关动作用CharacterMoveManagement.cs中的函数
            
            string[] responseStringList = responseString.Split('\n');
            bool isFound = false;
            string action = "";
            string target = "";
            foreach (string response in responseStringList)
            {
                if (response == "")
                {
                    continue;
                }
                string[] responseList = response.Split(',');
                string name = responseList[0];
                if (name == myself.name)
                {
                    isFound = true;
                    Debug.Log(myself.name + "is found");
                    action = responseList[1];
                    target = responseList[2];
                    break;
                }
            }
            GameObject targetObject = GameObject.Find(target);
            CharacterMoveManagement characterMoveManagement = this.GetComponent<CharacterMoveManagement>();
            switch (action)
            {
                case "撤退": //前往随机城镇
                    characterMoveManagement.moveToRandomTown();
                    Debug.Log("move to random town");
                    break;
                case "结盟":
                    characterMoveManagement.allianceWith(targetObject);
                    Debug.Log("alliance");
                    break;
                case "攻击":
                    characterMoveManagement.attack(targetObject);
                    Debug.Log("attack");
                    break;
                case "佯攻":
                    characterMoveManagement.pretendAttack(targetObject);
                    Debug.Log("pretend attack");
                    break;
                case "防守":
                    characterMoveManagement.stayForSecond();
                    Debug.Log("defend");
                    break;
                case "叛变":
                    characterMoveManagement.betrayTo(targetObject);
                    Debug.Log("betray");
                    break;
                case "移动": // 前往特定城镇
                    characterMoveManagement.moveTowards(targetObject);
                    Debug.Log("move");
                    break;
                default:
                    Debug.Log("no action");
                    break;
            }
            string actionToEng = "。。。。。。";
            switch (action)
            {
                case "撤退":
                    actionToEng = "retreat";
                    break;
                case "结盟":
                    actionToEng = "alliance";
                    break;
                case "攻击":
                    actionToEng = "attack";
                    break;
                case "佯攻":
                    actionToEng = "pretend attack";
                    break;
                case "防守":
                    actionToEng = "defend";
                    break;
                case "叛变":
                    actionToEng = "betray";
                    break;
                case "移动":
                    actionToEng = "move";
                    break;
                default:
                    break;
            }
            //message use TMP
            message.GetComponent<TextMeshPro>().text = actionToEng;
        }
        isChoice = false;
        Move();

        
    }
    private void Move()
    {
        // 获取所有在检测范围内的对象
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack);
        // 过滤出标签为enemyTag的对象
        Collider2D[] enemiesInView = new Collider2D[0]; // 初始化为空数组
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag(enemyTag1) && collider.gameObject.CompareTag(enemyTag2) && collider.gameObject.CompareTag(enemyTag3))
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
            //// 如果没有移动方向，默认的Idle状态
            //animator.SetBool("IsWalkingLeft", false);
            //animator.SetBool("IsWalkingUp", false);
            //animator.SetBool("IsWalkingDown", false);
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
        if (Vector2.Distance(transform.position, mPathPointList[mCurrentIndex]) > 1f)
        {
            Vector3 dir = (mPathPointList[mCurrentIndex] - transform.position).normalized;  //方向
            transform.position += dir * Time.deltaTime * speed;
            //Vector2 dir = (mPathPointList[mCurrentIndex] - transform.position).normalized;
            //float moveDistance = Time.deltaTime * speed;
            //transform.position = Vector2.MoveTowards(transform.position, mPathPointList[mCurrentIndex], moveDistance);

            // 根据移动方向设置Animator参数和镜像
            if (dir.x >= 0 )
            {
                // 向右走，使用IsWalkingLeft动画并通过镜像实现
                //animator.SetBool("IsWalkingLeft", true);
                spumPrefabs.PlayAnimation(1);
                MirrorCharacter(true);
            }
            else if (dir.x < 0 )
            {
                // 向左走，直接使用IsWalkingLeft动画
                //animator.SetBool("IsWalkingLeft", true);
                spumPrefabs.PlayAnimation(1);
                MirrorCharacter(false);
            }
            //else if (dir.y > 0 && Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
            //{
            //    animator.SetBool("IsWalkingUp", true);
            //    animator.SetBool("IsWalkingLeft", false);
            //}
            //else if (dir.y < 0 && Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
            //{
            //    animator.SetBool("IsWalkingDown", true);
            //    animator.SetBool("IsWalkingLeft", false);
            //}
            //else
            //{
            //    // 如果没有移动方向，默认的Idle状态
            //    //animator.SetBool("IsWalkingLeft", false);
            //    //animator.SetBool("IsWalkingUp", false);
            //    //animator.SetBool("IsWalkingDown", false);
            //    spumPrefabs.PlayAnimation(0);
            //}


        }
        else      //到达目标点
        {
            if (mCurrentIndex == mPathPointList.Count - 1)
            {
                // 如果没有移动方向，默认的Idle状态
                //animator.SetBool("IsWalkingLeft", false);
                //animator.SetBool("IsWalkingUp", false);
                //animator.SetBool("IsWalkingDown", false);
                spumPrefabs.PlayAnimation(0);

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
            //镜像name
            name.transform.localScale = new Vector3(-Mathf.Abs(name.transform.localScale.x), name.transform.localScale.y, name.transform.localScale.z);
            //镜像message，message用的是rect transform
            message.transform.localScale = new Vector3(-Mathf.Abs(message.transform.localScale.x), message.transform.localScale.y, message.transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            //镜像name
            name.transform.localScale = new Vector3(Mathf.Abs(name.transform.localScale.x), name.transform.localScale.y, name.transform.localScale.z);
            //镜像message
            message.transform.localScale = new Vector3(Mathf.Abs(message.transform.localScale.x), message.transform.localScale.y, message.transform.localScale.z);

        }
    }
    bool IsIdle()                                  //判断静止
    {
        //float movementThreshold = 0f; //速度判断阈值
        // 通过Rigidbody2D的velocity属性来判断角色是否静止
        return rb.velocity.sqrMagnitude <= 0;
    }
    void HandleEnemies(Collider2D[] enemies)  //周围有无敌人+处理方式
    {
        foreach (var enemy in enemies)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            // 根据方向设置攻击动画参数
            //if (direction.y > 0 && Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) // 敌人在上方
            //{
            //    animator.SetBool("IsAttackingUp", true);
            //    animator.SetBool("IsAttackingDown", false);
            //    animator.SetBool("IsAttackingLeft", false);
            //}
            //else if (direction.y < 0 && Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) // 敌人在下方
            //{
            //    animator.SetBool("IsAttackingUp", false);
            //    animator.SetBool("IsAttackingDown", true);
            //    animator.SetBool("IsAttackingLeft", false);
            //}
            if (direction.x >= 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // 敌人在右侧
            {
                //animator.SetBool("IsAttackingUp", false);
                //animator.SetBool("IsAttackingDown", false);
                //animator.SetBool("IsAttackingLeft", true);
                spumPrefabs.PlayAnimation(4);
                MirrorCharacter(true);
            }

            else if (direction.x < 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // 敌人在左侧
            {
                //animator.SetBool("IsAttackingUp", false);
                //animator.SetBool("IsAttackingDown", false);
                //animator.SetBool("IsAttackingLeft", true);
                spumPrefabs.PlayAnimation(4);
                MirrorCharacter(false);
            }
        }
    }
    //获取gpt.choice()的返回值

    private bool isChoice = false;
    private async void gptChoice()
    {
        isChoice = true;
        count = 0;
        myself.changeSurroundings(personas);
        string prompt_input = promptGenerate.ReadTextFile(".\\战役背景\\荆州之战.txt") + promptGenerate.create_persona_choice_prompt(myself);
        Debug.Log("prompt imput: " + prompt_input);
        gpt.setPrompt(prompt_input);
        responseString = await gpt.choice();
    } 
}
