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
    public bool idle = true;

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
    // Start is called before the first frame update
    void Start() 
    {
        //randomly set a target
        this.gameObject.GetComponent<NewBehaviourScript>().SetSelected(false);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void escapeFrom (Vector3 playerPosition)
    {
        // Move away from the player
        // Select a random position away from the player
        Vector3 direction = transform.position - playerPosition;
        direction.Normalize();
        playerPosition += direction * direction.magnitude * 2;
        this.gameObject.GetComponent<NewBehaviourScript>().targetPosition = playerPosition;
    }

    void escapeFrom(GameObject player)
    {
        // Move away from the player
        Vector3 direction = transform.position - player.transform.position;
        direction.Normalize();
        Vector3 playerPosition = player.transform.position + direction * direction.magnitude * 2;
        this.gameObject.GetComponent<NewBehaviourScript>().targetPosition = playerPosition;
    }

    void moveTowards(Vector3 playerPosition)
    {
        // Move towards the player
        this.gameObject.GetComponent<NewBehaviourScript>().targetPosition = playerPosition;
    }

    void moveTowards(GameObject player)
    {
        // Move towards the player
        this.gameObject.GetComponent<NewBehaviourScript>().targetPosition = player.transform.position;
    }
}
