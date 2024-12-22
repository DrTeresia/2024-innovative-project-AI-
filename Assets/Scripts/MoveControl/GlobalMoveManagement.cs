using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalMoveManagement : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject[] allPlayer;
    private double upperBound = 40;
    private double lowerBound = -60;
    private double leftBound = -75;
    private double rightBound = 75;

    public enum MoveType
    {
        move,
        idle
    }
    public enum ActionType
    {
        attack,
        conversation,
        stay,
        idle
    }

    public enum Camp
    {
        ����Ӫ,
        �ܲ�,
        ����,
        Ԭ��,
        Ԭ��,
        ��׿,
        ��ǫ,
        ̣�ٵ���,
        ���￵,
        ���,
        ����,
        �ϰ׻�,
        ����,
        ��ۣ,
        ½��Ⱥ,
        �ƽ��
    }
    public MoveType move;
    public ActionType action;
    public List<GameObject>[] camp;
    public string mapName;

    void Start()
    {
        allPlayer = GameObject.FindGameObjectsWithTag("Player");
        mapName = GameObject.Find("MapName").GetComponent<Text>().text;

        camp = new List<GameObject>[15];
        for (int i = 0; i < 15; i++)
        {
            camp[i] = new List<GameObject>();
        }
        switch (mapName)
        {
            case "North":
                
                foreach (GameObject player in allPlayer)
                {
                    if (player.GetComponent<CharacterMoveManagement>().camp == Camp.�ܲ�)
                    {
                        camp[(int)Camp.�ܲ�].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.����)
                    {
                        camp[(int)Camp.����].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.Ԭ��)
                    {
                        camp[(int)Camp.Ԭ��].Add(player);
                    }
                }
                break;
            case "JiangDong":
                foreach (GameObject player in allPlayer)
                {
                    if (player.GetComponent<CharacterMoveManagement>().camp == Camp.���)
                    {
                        camp[(int)Camp.���].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.½��Ⱥ)
                    {
                        camp[(int)Camp.½��Ⱥ].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.�ƽ��)
                    {
                        camp[(int)Camp.�ƽ��].Add(player);
                    }
                }
                break;
            case "JinZhou":
                foreach (GameObject player in allPlayer)
                {
                    if (player.GetComponent<CharacterMoveManagement>().camp == Camp.Ԭ��)
                    {
                        camp[(int)Camp.Ԭ��].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.��׿)
                    {
                        camp[(int)Camp.��׿].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.̣�ٵ���)
                    {
                        camp[(int)Camp.̣�ٵ���].Add(player);
                    }
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool isEliminate(GameObject attacker, GameObject underAttacker)
    {
        bool isEliminate = false;
        bool isWake = false;
        bool isUnderAttack = false;
        bool isSurrender = false;
        if (isWake && isUnderAttack)
        {
            isEliminate = true;
        }
        else if (isSurrender)
        {
            isEliminate = true;
        }
        return isEliminate;
    }

    public void groupAttack(Camp camp, GameObject underAttacker)
    {
        foreach (GameObject player in this.camp[(int)camp])
        {
            singleAttack(player, underAttacker);
        }
    }
    public void singleAttack(GameObject attacker, GameObject underAttacker)
    {
        attacker.GetComponent<CharacterMoveManagement>().moveTowards(underAttacker);
        attacker.GetComponent<CharacterMoveManagement>().actionStatus = ActionType.attack;
    }

    
}


