using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
      
        Destroy(gameObject); // ���ٵ�λ��GameObject
    }

    void Start()
    {
        currentHealth = maxHealth;
    }
}
