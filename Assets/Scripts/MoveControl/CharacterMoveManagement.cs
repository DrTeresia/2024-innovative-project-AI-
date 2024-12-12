using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveManagement : MonoBehaviour
{
    
    // Load self game object
    private double upperBound = 40;
    private double lowerBound = -60;
    private double leftBound = -75;
    private double rightBound = 75;
    private float DISTANCE_TOLERANCE = 1.0f;

    public Vector3 targetPosition;
    public bool isIdle = true;
    public GlobalMoveManagement.MoveType moveStatus;
    public GlobalMoveManagement.ActionType actionStatus;
    public GlobalMoveManagement.Camp camp;


    // Start is called before the first frame update
    void Start() 
    {
        //randomly set a target
        this.gameObject.GetComponent<NewBehaviourScript>().SetSelected(false);     
        moveStatus = GlobalMoveManagement.MoveType.idle;
        actionStatus = GlobalMoveManagement.ActionType.idle;
    }

    // Update is called once per frame
    void Update()
    {
        //if the object's position is close to the target position, set the object to idle
        if (GlobalMoveManagement.MoveType.move == moveStatus && Vector3.Distance(targetPosition, transform.position) < DISTANCE_TOLERANCE)
        {
            moveStatus = GlobalMoveManagement.MoveType.idle;
            isIdle = true;
        }

    }

    public void escapeFrom (Vector3 playerPosition)
    {
        // Move away from the player
        // Select a random position away from the player
        Vector3 direction = transform.position - playerPosition;
        direction.Normalize();
        playerPosition += direction * direction.magnitude * 2;
        this.gameObject.GetComponent<NewBehaviourScript>().targetPosition = playerPosition;
    }

    public void escapeFrom(GameObject player)
    {
        // Move away from the player
        Vector3 direction = transform.position - player.transform.position;
        direction.Normalize();
        Vector3 playerPosition = player.transform.position + direction * direction.magnitude * 2;
        this.gameObject.GetComponent<NewBehaviourScript>().targetPosition = playerPosition;
    }

    public void moveTowards(Vector3 playerPosition)
    {
        // Move towards the player
        this.gameObject.GetComponent<NewBehaviourScript>().targetPosition = playerPosition;
    }

    public void moveTowards(GameObject player)
    {
        // Move towards the player
        this.gameObject.GetComponent<NewBehaviourScript>().targetPosition = player.transform.position;
    }
}
