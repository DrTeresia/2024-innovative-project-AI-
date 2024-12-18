using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitHealthAndAttack : MonoBehaviour
{
    public int maxHealth = 100; // �������ֵ
    public int currentHealth; // ��ǰ����ֵ
    public int attackDamage = 10; // �����˺�
    public float attackRange = 1f; // ������Χ
    public float attackInterval = 1f; // �������ʱ��
    public string FriendTag; // ���˵ı�ǩ
    private float nextAttackTime = 0f; // �´ι�����ʱ��
    private Rigidbody2D rb; // �����ƶ���Rigidbody2D���

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Time.time > nextAttackTime && IsIdle())
        {
            // ��ⷶΧ�ڵĵ���
            List<Transform> enemiesInRange = new List<Transform>();
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (var enemy in enemies)
            {
                if (!enemy.gameObject.CompareTag(FriendTag) && enemy.gameObject != gameObject)
                {
                    enemiesInRange.Add(enemy.transform);
                }
            }

            // �����Χ���е��ˣ���������һ��
            if (enemiesInRange.Count > 0)
            {
                AttackNearestEnemy(enemiesInRange[0]);
                nextAttackTime = Time.time + attackInterval; // �����´ι���ʱ��
            }
        }
    }

    void AttackNearestEnemy(Transform enemy)
    {
        // �������Ҳ������ű���ֱ�ӵ��õ��˵�TakeDamage����
        UnitHealthAndAttack enemyHealth = enemy.GetComponent<UnitHealthAndAttack>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
        }
    }

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
        Destroy(gameObject);
        // ������������Ӷ���������߼����粥�Ŷ��������ɵ�����ȡ�
    }
    bool IsIdle()                                  //�жϾ�ֹ
    {
        // ͨ��Rigidbody2D��velocity�������жϽ�ɫ�Ƿ�ֹ
        return rb.velocity.sqrMagnitude <= 0;
    }
}
