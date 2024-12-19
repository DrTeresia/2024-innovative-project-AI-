using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ClickToNorth : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // When the object is clicked, the scene will switch to NorthUnifyBattle
    void OnMouseDown()
    {
        SceneManager.LoadScene("NorthUnifyBattle");
    }

}
