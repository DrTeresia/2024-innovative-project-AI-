using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMovement : MonoBehaviour
{
    public GameObject Panel; // DirectGenMap is in panel
    public DirectGenMap directGenMap;
    public Move move; // Move is in self object
    void Start()
    {
        directGenMap = Panel.GetComponent<DirectGenMap>();
        move = GetComponent<Move>();
    }

    void Update()
    {
        
    }
}

