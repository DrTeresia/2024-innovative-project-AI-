using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该组件用于控制单个角色的移动，与GlobalMoveManagement组件配合使用
//该组件需要挂载在角色的GameObject上
//角色的行为分为两个维度，一个是移动，一个是动作，分别对应moveStatus和actionStatus
//角色的阵营由camp决定，camp由GlobalMoveManagement组件设置
//角色的目标位置由targetPosition决定，targetPosition调用了A_Patn中的功能

/*角色的移动状态分为三种：
 * idle：空闲状态，不移动
 * move：移动状态，朝着targetPosition移动
 * leave：离开状态，朝着targetPosition移动
 * 
 * 当角色的isIdle为true时，向DeepSeek询问下一步动作
 */

public class CharacterMoveManagement : MonoBehaviour
{

    // Range of map
    private double upperBound = 40;
    private double lowerBound = -60;
    private double leftBound = -75;
    private double rightBound = 75;

    //stay timer
    private float maxStayTime = 2.0f;
    private float stayStartTime = 0.0f;

    //constant values
    private float DISTANCE_TOLERANCE = 1.0f;
    private float RANGE_OF_VIEW = 5.0f;

    //global infomation
    public static GameObject[] players;
    public static GameObject[] towns;

    public Vector3 targetPosition;
    public bool isIdle = true;
    public GlobalMoveManagement.MoveType moveStatus;
    public GlobalMoveManagement.ActionType actionStatus;
    public GlobalMoveManagement.Camp camp;


    // Start is called before the first frame update
    void Awake()
    {
        //load players
        players = GameObject.FindGameObjectsWithTag("Player");
        //load towns
        towns = GameObject.FindGameObjectsWithTag("Town");

    }
    void Start()
    {
        //randomly set a target
        moveStatus = GlobalMoveManagement.MoveType.idle;
        actionStatus = GlobalMoveManagement.ActionType.idle;

        camp = GlobalMoveManagement.Camp.无阵营;

    }

    // Update is called once per frame
    void Update()
    {
        
        //if the object's position is close to the target position, set the moveAction to idle
        if (GlobalMoveManagement.MoveType.move == moveStatus && Vector2.Distance(targetPosition, transform.position) < DISTANCE_TOLERANCE)
        {
            moveStatus = GlobalMoveManagement.MoveType.idle;
        }
        //if the object's stay time is than maxStayTime, set the actionStatus to idle
        if (GlobalMoveManagement.MoveType.idle == moveStatus && Time.time - stayStartTime > maxStayTime)
        {
            actionStatus = GlobalMoveManagement.ActionType.idle;
        }
        //if the object's is close to the target position, and the action is idle, set the isIdle to true
        if (GlobalMoveManagement.ActionType.idle == actionStatus && GlobalMoveManagement.MoveType.idle == moveStatus)
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
        }

        //if the object is idle, ask DeepSeek for the next action
        //if (isIdle)
        //{
        //    //select a random target position
        //    int index = Random.Range(0, towns.Length + players.Length);
        //    if (index < towns.Length)
        //    {
        //        targetPosition = towns[index].transform.position;
        //    }
        //    else
        //    {
        //        targetPosition = players[index - towns.Length].transform.position;
        //    }
        //    moveTowards(targetPosition);
        //}
    }

    public void escapeFrom(Vector3 playerPosition)
    {
        // Move away from the player
        // Select a random position away from the player
        Vector3 direction = transform.position - playerPosition;
        direction.Normalize();
        playerPosition += direction * direction.magnitude * 2;
        this.gameObject.GetComponent<Movement>().targetPosition = playerPosition;
        moveStatus = GlobalMoveManagement.MoveType.move;
    }

    public void escapeFrom(GameObject player)
    {
        // Move away from the player
        Vector3 direction = transform.position - player.transform.position;
        direction.Normalize();
        Vector3 playerPosition = player.transform.position + direction * direction.magnitude * 2;
        this.gameObject.GetComponent<Movement>().targetPosition = playerPosition;
        moveStatus = GlobalMoveManagement.MoveType.move;
    }

    public void moveTowards(Vector3 playerPosition)
    {
        // Move towards the player
        this.gameObject.GetComponent<Movement>().targetPosition = playerPosition;
        moveStatus = GlobalMoveManagement.MoveType.move;
    }

    public void moveTowards(GameObject player)
    {
        // Move towards the player
        this.gameObject.GetComponent<Movement>().targetPosition = player.transform.position;
        moveStatus = GlobalMoveManagement.MoveType.move;
    }

    public void stayForSecond(float time = 3.0f)
    {
        stayStartTime = Time.time;
        maxStayTime = time;
        moveStatus = GlobalMoveManagement.MoveType.idle;
        actionStatus = GlobalMoveManagement.ActionType.stay;
    }

    //佯攻，朝着playerPosition移动，但是停在playerPosition的前方
    public void pretendAttack(Vector3 playerPosition)
    {
        // Pretend to attack the player
        Vector3 direction = transform.position - playerPosition;
        direction.Normalize();
        Vector3 playerPositionFront = playerPosition + direction * 2;
        this.gameObject.GetComponent<Movement>().targetPosition = playerPositionFront;
        moveStatus = GlobalMoveManagement.MoveType.move;
        actionStatus = GlobalMoveManagement.ActionType.stay;
    }
    public void pretendAttack(GameObject player)
    {
        // Pretend to attack the player
        Vector3 direction = transform.position - player.transform.position;
        direction.Normalize();
        Vector3 playerPositionFront = player.transform.position + direction * 2;
        this.gameObject.GetComponent<Movement>().targetPosition = playerPositionFront;
        moveStatus = GlobalMoveManagement.MoveType.move;
        actionStatus = GlobalMoveManagement.ActionType.stay;
    }
    public void allianceWith(GameObject player)
    {
        // Pretend to attack the player
        Vector3 direction = transform.position - player.transform.position;
        direction.Normalize();
        Vector3 playerPositionFront = player.transform.position + direction * 2;
        this.gameObject.GetComponent<Movement>().targetPosition = playerPositionFront;
        moveStatus = GlobalMoveManagement.MoveType.move;
        actionStatus = GlobalMoveManagement.ActionType.conversation;
    }
    public void attack(GameObject player)
    {
        // Attack the player
        this.gameObject.GetComponent<Movement>().targetPosition = player.transform.position;
        moveStatus = GlobalMoveManagement.MoveType.move;
        actionStatus = GlobalMoveManagement.ActionType.attack;
    }
    public void betrayTo(GameObject player)
    {
        // Change self's camp to the player's camp, 通过修改tag来实现
        this.gameObject.tag = player.tag;
    }
}
