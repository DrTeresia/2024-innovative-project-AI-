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

    void Start()
    {
        allPlayer = GameObject.FindGameObjectsWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

}
