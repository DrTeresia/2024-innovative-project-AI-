using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCamp : MonoBehaviour
{

    public Camera mainCamera;
    private bool isShow = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isShow = !isShow;
            if (isShow)
            {
                mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Camp");
            }
            else
            {
                mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Camp"));
            }
        }
    }    
}
