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
        ÎÞÕóÓª,
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
                    if (player.GetComponent<CharacterMoveManagement>().camp == Camp.²Ü²Ù)
                    {
                        camp[(int)Camp.²Ü²Ù].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.ÂÀ²¼)
                    {
                        camp[(int)Camp.ÂÀ²¼].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.Ô¬ÉÜ)
                    {
                        camp[(int)Camp.Ô¬ÉÜ].Add(player);
                    }
                }
                break;
            case "JiangDong":
                foreach (GameObject player in allPlayer)
                {
                    if (player.GetComponent<CharacterMoveManagement>().camp == Camp.Ëï²ß)
                    {
                        camp[(int)Camp.Ëï²ß].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.Â½»ëÈº)
                    {
                        camp[(int)Camp.Â½»ëÈº].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.»Æ½í¾ü)
                    {
                        camp[(int)Camp.»Æ½í¾ü].Add(player);
                    }
                }
                break;
            case "JinZhou":
                foreach (GameObject player in allPlayer)
                {
                    if (player.GetComponent<CharacterMoveManagement>().camp == Camp.Ô¬Êõ)
                    {
                        camp[(int)Camp.Ô¬Êõ].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.¶­×¿)
                    {
                        camp[(int)Camp.¶­×¿].Add(player);
                    }
                    else if (player.GetComponent<CharacterMoveManagement>().camp == Camp.Ì£¶Ùµ¥ÓÚ)
                    {
                        camp[(int)Camp.Ì£¶Ùµ¥ÓÚ].Add(player);
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

