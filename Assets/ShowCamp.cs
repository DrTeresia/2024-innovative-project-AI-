using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCamp : MonoBehaviour
{

    public Camera mainCamera;
    private bool isShow = false;

    public GameObject Map;
    public GameObject Camp;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isShow = !isShow;
            if (isShow)
            {
                mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Camp");
                // ����Map͸����Ϊ0.5f
                Map.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Camp"));
                // �ָ�Map͸����Ϊ1.0f
                Map.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1.0f);
            }
        }
    }    
}
