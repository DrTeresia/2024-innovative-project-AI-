using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterGen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateCharacter()
    {
        // Generate a character
        // 1. Name
        //
        GameObject mainCharacter = Instantiate(Resources.Load("general"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    }
}

