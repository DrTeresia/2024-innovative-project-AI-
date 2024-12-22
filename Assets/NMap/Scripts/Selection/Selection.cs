using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject selectoinPrefeb;
    [SerializeField]
    private GameObject newSelection;

    public bool isSelected;
    //�״ε���л���ѡ��״̬
    private void OnMouseDown()
    {
        
        if (newSelection == null)
        {
            newSelection = Instantiate(selectoinPrefeb, transform.position, Quaternion.identity) as GameObject;
            newSelection.transform.SetParent(gameObject.transform);
            newSelection.SetActive(true);
        }
    }
    
}
