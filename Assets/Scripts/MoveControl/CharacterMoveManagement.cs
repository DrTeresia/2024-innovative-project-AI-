using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������ڿ��Ƶ�����ɫ���ƶ�����GlobalMoveManagement������ʹ��
//�������Ҫ�����ڽ�ɫ��GameObject��
//��ɫ����Ϊ��Ϊ����ά�ȣ�һ�����ƶ���һ���Ƕ������ֱ��ӦmoveStatus��actionStatus
//��ɫ����Ӫ��camp������camp��GlobalMoveManagement�������
//��ɫ��Ŀ��λ����targetPosition������targetPosition������A_Patn�еĹ���

/*��ɫ���ƶ�״̬��Ϊ���֣�
 * idle������״̬�����ƶ�
 * move���ƶ�״̬������targetPosition�ƶ�
 * leave���뿪״̬������targetPosition�ƶ�
 * 
 * ����ɫ��isIdleΪtrueʱ����DeepSeekѯ����һ������
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
        this.gameObject.GetComponent<A_Path>().SetSelected(false);
        moveStatus = GlobalMoveManagement.MoveType.idle;
        actionStatus = GlobalMoveManagement.ActionType.idle;

        camp = GlobalMoveManagement.Camp.����Ӫ;

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
        if (isIdle)
        {
            //select a random target position
            int index = Random.Range(0, towns.Length + players.Length);
            if (index < towns.Length)
            {
                targetPosition = towns[index].transform.position;
            }
            else
            {
                targetPosition = players[index - towns.Length].transform.position;
            }
            moveTowards(targetPosition);
        }
    }

    public void escapeFrom(Vector3 playerPosition)
    {
        // Move away from the player
        // Select a random position away from the player
        Vector3 direction = transform.position - playerPosition;
        direction.Normalize();
        playerPosition += direction * direction.magnitude * 2;
        this.gameObject.GetComponent<A_Path>().targetPosition = playerPosition;
        moveStatus = GlobalMoveManagement.MoveType.move;
    }

    public void escapeFrom(GameObject player)
    {
        // Move away from the player
        Vector3 direction = transform.position - player.transform.position;
        direction.Normalize();
        Vector3 playerPosition = player.transform.position + direction * direction.magnitude * 2;
        this.gameObject.GetComponent<A_Path>().targetPosition = playerPosition;
        moveStatus = GlobalMoveManagement.MoveType.move;
    }

    public void moveTowards(Vector3 playerPosition)
    {
        // Move towards the player
        this.gameObject.GetComponent<A_Path>().targetPosition = playerPosition;
        moveStatus = GlobalMoveManagement.MoveType.move;
    }

    public void moveTowards(GameObject player)
    {
        // Move towards the player
        this.gameObject.GetComponent<A_Path>().targetPosition = player.transform.position;
        moveStatus = GlobalMoveManagement.MoveType.move;
    }

    private void isMeet(GameObject player)
    {
        // Check if the player is close to the object
        if (Vector2.Distance(player.transform.position, transform.position) < RANGE_OF_VIEW)
        {
            // If the player is close, ask GPT what to do.
            //GlobalMoveManagement.ActionType action = GlobalMoveManagement.ActionType.idle;
        }
    }
}