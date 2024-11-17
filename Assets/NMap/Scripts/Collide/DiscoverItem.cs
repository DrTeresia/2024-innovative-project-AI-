using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
//���������ŵ����������ϣ�������ҽ��������������ķ�Χʱ��������ʾ������Ϣ
public class DiscoverItem : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionPrefeb;

    private GameObject newSelection;

    public bool isSelected;
    //������ҽ��������������ķ�Χʱ��������ʾ������Ϣ
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
    //��������뿪�����������ķ�Χʱ�����������������Ϣ
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
