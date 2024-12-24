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
    private List<Vector3> mPathPointList;  //Ŀ���λ
    private int mCurrentIndex = 0;
    public float speed;
    private Animator animator;// Animator�������
    private Rigidbody2D rb; // �����ƶ���Rigidbody2D���
    //public LayerMask enemyLayer; // �������ڵ�LayerMask
    public string enemyTag1; // ���˵ı�ǩ
    public string enemyTag2;
    public string enemyTag3;
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // �������ĵ��˼���
    public float detectionRange_of_attack; // ��ⷶΧ

    public Vector3 targetPosition; // Ŀ��λ�ã��������룩

    private SPUM_Prefabs spumPrefabs; // ���ƶ����Ľű�����

    private GameObject name;
    private string nameString;
    private GameObject message;

    //gpt���
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
        //enemyLayer = LayerMask.NameToLayer("enemy");           //ͼ��Ϊenemy
        //enemyTag = "enemy";                     //��ǩΪenemy
        // ��ȡSPUM_Prefabs�ű�������
        spumPrefabs = GetComponent<SPUM_Prefabs>();
        //gpt���
        myself = new Persona(gameObject.name);
        Debug.Log("myself: " + myself.name);

        //�ҵ�����name
        name = transform.Find("name").gameObject;
        nameString = name.GetComponent<TextMesh>().text;

        message = transform.Find("message").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(1))     //�Ҽ�ȡ����ѡ�ı���
        //{
        //    isSelected = false;
        //}

        ////����״̬���
        //animator.SetBool("IsAttackingUp", false);
        //animator.SetBool("IsAttackingDown", false);
        //animator.SetBool("IsAttackingLeft", false);

        //if (!isSelected)
        //{
        //    return;
        //}

        //if (Input.GetMouseButtonDown(0))              //�������
        //{
        //    ////����������λ��ʱ���Ȱ�λ������
        //    //animator.SetBool("IsWalkingLeft", false);
        //    //animator.SetBool("IsWalkingUp", false);
        //    //animator.SetBool("IsWalkingDown", false);

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
        count++;
        int randomWaitingTime = Random.Range(300, 600);
        if (count >= randomWaitingTime && !isChoice)
        {
            gptChoice();
            count = 0;
            Debug.Log("todo: " + responseString);
            //��������responseString�ж��У��������̶���ÿ����һ�����
            //���ṹΪ��[��ɫ��],[����],[Ŀ��]
            //�����У����ˣ����ˣ��������𹥣�����
            //��ض�����CharacterMoveManagement.cs�еĺ���
            
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
                case "����": //ǰ���������
                    characterMoveManagement.moveToRandomTown();
                    Debug.Log("move to random town");
                    break;
                case "����":
                    characterMoveManagement.allianceWith(targetObject);
                    Debug.Log("alliance");
                    break;
                case "����":
                    characterMoveManagement.attack(targetObject);
                    Debug.Log("attack");
                    break;
                case "��":
                    characterMoveManagement.pretendAttack(targetObject);
                    Debug.Log("pretend attack");
                    break;
                case "����":
                    characterMoveManagement.stayForSecond();
                    Debug.Log("defend");
                    break;
                case "�ѱ�":
                    characterMoveManagement.betrayTo(targetObject);
                    Debug.Log("betray");
                    break;
                case "�ƶ�": // ǰ���ض�����
                    characterMoveManagement.moveTowards(targetObject);
                    Debug.Log("move");
                    break;
                default:
                    Debug.Log("no action");
                    break;
            }
            string actionToEng = "������������";
            switch (action)
            {
                case "����":
                    actionToEng = "retreat";
                    break;
                case "����":
                    actionToEng = "alliance";
                    break;
                case "����":
                    actionToEng = "attack";
                    break;
                case "��":
                    actionToEng = "pretend attack";
                    break;
                case "����":
                    actionToEng = "defend";
                    break;
                case "�ѱ�":
                    actionToEng = "betray";
                    break;
                case "�ƶ�":
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
        // ��ȡ�����ڼ�ⷶΧ�ڵĶ���
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange_of_attack);
        // ���˳���ǩΪenemyTag�Ķ���
        Collider2D[] enemiesInView = new Collider2D[0]; // ��ʼ��Ϊ������
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag(enemyTag1) && collider.gameObject.CompareTag(enemyTag2) && collider.gameObject.CompareTag(enemyTag3))
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
            //// ���û���ƶ�����Ĭ�ϵ�Idle״̬
            //animator.SetBool("IsWalkingLeft", false);
            //animator.SetBool("IsWalkingUp", false);
            //animator.SetBool("IsWalkingDown", false);
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
        if (Vector2.Distance(transform.position, mPathPointList[mCurrentIndex]) > 1f)
        {
            Vector3 dir = (mPathPointList[mCurrentIndex] - transform.position).normalized;  //����
            transform.position += dir * Time.deltaTime * speed;
            //Vector2 dir = (mPathPointList[mCurrentIndex] - transform.position).normalized;
            //float moveDistance = Time.deltaTime * speed;
            //transform.position = Vector2.MoveTowards(transform.position, mPathPointList[mCurrentIndex], moveDistance);

            // �����ƶ���������Animator�����;���
            if (dir.x >= 0 )
            {
                // �����ߣ�ʹ��IsWalkingLeft������ͨ������ʵ��
                //animator.SetBool("IsWalkingLeft", true);
                spumPrefabs.PlayAnimation(1);
                MirrorCharacter(true);
            }
            else if (dir.x < 0 )
            {
                // �����ߣ�ֱ��ʹ��IsWalkingLeft����
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
            //    // ���û���ƶ�����Ĭ�ϵ�Idle״̬
            //    //animator.SetBool("IsWalkingLeft", false);
            //    //animator.SetBool("IsWalkingUp", false);
            //    //animator.SetBool("IsWalkingDown", false);
            //    spumPrefabs.PlayAnimation(0);
            //}


        }
        else      //����Ŀ���
        {
            if (mCurrentIndex == mPathPointList.Count - 1)
            {
                // ���û���ƶ�����Ĭ�ϵ�Idle״̬
                //animator.SetBool("IsWalkingLeft", false);
                //animator.SetBool("IsWalkingUp", false);
                //animator.SetBool("IsWalkingDown", false);
                spumPrefabs.PlayAnimation(0);

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
            //����name
            name.transform.localScale = new Vector3(-Mathf.Abs(name.transform.localScale.x), name.transform.localScale.y, name.transform.localScale.z);
            //����message��message�õ���rect transform
            message.transform.localScale = new Vector3(-Mathf.Abs(message.transform.localScale.x), message.transform.localScale.y, message.transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            //����name
            name.transform.localScale = new Vector3(Mathf.Abs(name.transform.localScale.x), name.transform.localScale.y, name.transform.localScale.z);
            //����message
            message.transform.localScale = new Vector3(Mathf.Abs(message.transform.localScale.x), message.transform.localScale.y, message.transform.localScale.z);

        }
    }
    bool IsIdle()                                  //�жϾ�ֹ
    {
        //float movementThreshold = 0f; //�ٶ��ж���ֵ
        // ͨ��Rigidbody2D��velocity�������жϽ�ɫ�Ƿ�ֹ
        return rb.velocity.sqrMagnitude <= 0;
    }
    void HandleEnemies(Collider2D[] enemies)  //��Χ���޵���+����ʽ
    {
        foreach (var enemy in enemies)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            // ���ݷ������ù�����������
            //if (direction.y > 0 && Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) // �������Ϸ�
            //{
            //    animator.SetBool("IsAttackingUp", true);
            //    animator.SetBool("IsAttackingDown", false);
            //    animator.SetBool("IsAttackingLeft", false);
            //}
            //else if (direction.y < 0 && Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) // �������·�
            //{
            //    animator.SetBool("IsAttackingUp", false);
            //    animator.SetBool("IsAttackingDown", true);
            //    animator.SetBool("IsAttackingLeft", false);
            //}
            if (direction.x >= 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // �������Ҳ�
            {
                //animator.SetBool("IsAttackingUp", false);
                //animator.SetBool("IsAttackingDown", false);
                //animator.SetBool("IsAttackingLeft", true);
                spumPrefabs.PlayAnimation(4);
                MirrorCharacter(true);
            }

            else if (direction.x < 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // ���������
            {
                //animator.SetBool("IsAttackingUp", false);
                //animator.SetBool("IsAttackingDown", false);
                //animator.SetBool("IsAttackingLeft", true);
                spumPrefabs.PlayAnimation(4);
                MirrorCharacter(false);
            }
        }
    }
    //��ȡgpt.choice()�ķ���ֵ

    private bool isChoice = false;
    private async void gptChoice()
    {
        isChoice = true;
        count = 0;
        myself.changeSurroundings(personas);
        string prompt_input = promptGenerate.ReadTextFile(".\\ս�۱���\\����֮ս.txt") + promptGenerate.create_persona_choice_prompt(myself);
        Debug.Log("prompt imput: " + prompt_input);
        gpt.setPrompt(prompt_input);
        responseString = await gpt.choice();
    } 
}
