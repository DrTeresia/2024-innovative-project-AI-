using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionToDestory : MonoBehaviour
{
    private void OnMouseDown()
    {
        Destroy(gameObject);
    }
}