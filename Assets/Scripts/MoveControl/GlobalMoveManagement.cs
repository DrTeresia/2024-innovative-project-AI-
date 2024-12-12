using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        leave
    }
    public enum ActionType
    {
        attack,
        idle
    }

    public enum Camp
    {
        ²Ü²Ù,
        ÂÀ²¼,
        Ô¬ÉÜ,
        Ô¬Êõ,
        ¶­×¿,
        ÌÕÇ«,
        Ì£¶Ùµ¥ÓÚ,
        ¹«Ëï¿µ,
        Ëï²ß,
        Áõôí,
        ÑÏ°×»¢,
        ÍõÀÊ,
        ÁºÛ£,
        Â½»ëÈº,
        »Æ½í¾ü
    }

    public MoveType move;
    public ActionType action;
    public List<GameObject>[] camp;
    public string mapName;

    void Start()
    {
        allPlayer = GameObject.FindGameObjectsWithTag("Player");
        mapName = GameObject.Find("MapName").GetComponent<Text>();

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
                    if (player.GetComponent<NewBehaviourScript>().camp == Camp.²Ü²Ù)
                    {
                        camp[Enum.GetValue(Camp.²Ü²Ù)].Add(player);
                    }
                    else if (player.GetComponent<NewBehaviourScript>().camp == Camp.ÂÀ²¼)
                    {
                        camp[Enum.GetValue(Camp.ÂÀ²¼)].Add(player);
                    }
                    else if (player.GetComponent<NewBehaviourScript>().camp == Camp.Ô¬ÉÜ)
                    {
                        camp[Enum.GetValue(Camp.Ô¬ÉÜ)].Add(player);
                    }
                }
                break;
            case "JiangDong":
                foreach (GameObject player in allPlayer)
                {
                    if (player.GetComponent<NewBehaviourScript>().camp == Camp.Ëï²ß)
                    {
                        camp[Camp.Ëï²ß.GetValue()].Add(player);
                    }
                    else if (player.GetComponent<NewBehaviourScript>().camp == Camp.Áõôí)
                    {
                        camp[Camp.Áõôí.GetValue()].Add(player);
                    }
                    else if (player.GetComponent<NewBehaviourScript>().camp == Camp.ÑÏ°×»¢)
                    {
                        camp[Camp.ÑÏ°×»¢.GetValue()].Add(player);
                    }
                }
            case "JinZhou":
                foreach (GameObject player in allPlayer)
                {
                    if (player.GetComponent<NewBehaviourScript>().camp == Camp.Ô¬Êõ)
                    {
                        camp[Camp.Ô¬Êõ.GetValue()].Add(player);
                    }
                    else if (player.GetComponent<NewBehaviourScript>().camp == Camp.¶­×¿)
                    {
                        camp[Camp.¶­×¿.GetValue()].Add(player);
                    }
                    else if (player.GetComponent<NewBehaviourScript>().camp == Camp.ÌÕÇ«)
                    {
                        camp[Camp.ÌÕÇ«.GetValue()].Add(player);
                    }
                }
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
        foreach (GameObject player in this.camp[Enum.GetValues(camp)])
        {
            if (isEliminate(player, underAttacker))
            {
                player.GetComponent<NewBehaviourScript>().actionStatus = ActionType.attack;
                player.GetComponent<NewBehaviourScript>().target = underAttacker;
            }
        }
    }

}


