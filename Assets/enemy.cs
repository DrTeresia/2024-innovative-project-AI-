using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : character
{
    public float damage;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<character>().TakeDamage(damage);
        }
    }

}
