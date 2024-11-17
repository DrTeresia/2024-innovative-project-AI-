using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
//这个组件被放到环境物体上，当有玩家进入这个环境物体的范围时，物体会揭示自身信息
public class DiscoverItem : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionPrefeb;

    private GameObject newSelection;

    public bool isSelected;
    //当有玩家进入这个环境物体的范围时，物体会揭示自身信息
    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.CompareTag("Player"))
        //{
            UnityEngine.Debug.Log("Player is in the range of " + gameObject.name);
            if (newSelection == null)
            {
                newSelection = Instantiate(selectionPrefeb, transform.position, Quaternion.identity);
                newSelection.transform.SetParent(transform);
            }
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
            UnityEngine.Debug.Log("Player is in the range of " + gameObject.name);
            if (newSelection == null)
            {
                newSelection = Instantiate(selectionPrefeb, transform.position, Quaternion.identity);
                newSelection.transform.SetParent(transform);
            }
        //}
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        //if (other.CompareTag("Player"))
        //{
            UnityEngine.Debug.Log("Player is in the range of " + gameObject.name);
            if (newSelection == null)
            {
                newSelection = Instantiate(selectionPrefeb, transform.position, Quaternion.identity);
                newSelection.transform.SetParent(transform);
            }
        //}
    }
    //当有玩家离开这个环境物体的范围时，物体会隐藏自身信息
    private void OnTriggerExit2D(Collider2D other)
    {
        //if (other.CompareTag("Player"))
        //{
            UnityEngine.Debug.Log("Player is out of the range of " + gameObject.name);
            if (newSelection != null)
            {
                Destroy(newSelection);
            }
        //}
    }

    private void OnMouseDown()
    {
        UnityEngine.Debug.Log("Player clicked on " + gameObject.name);
    }
}
