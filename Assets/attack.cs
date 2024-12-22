using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitHealthAndAttack : MonoBehaviour
{
    public int maxHealth = 100; // 最大生命值
    public int currentHealth; // 当前生命值
    public int attackDamage = 10; // 攻击伤害
    public float attackRange = 1f; // 攻击范围
    public float attackInterval = 1f; // 攻击间隔时间
    public string FriendTag; // 敌人的标签
    private float nextAttackTime = 0f; // 下次攻击的时间
    private Rigidbody2D rb; // 用于移动的Rigidbody2D组件
    private SPUM_Prefabs spumPrefabs; // 控制动画的脚本引用

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spumPrefabs = GetComponent<SPUM_Prefabs>();
    }

    void Update()
    {
        if (Time.time > nextAttackTime && IsIdle())
        {
            // 检测范围内的敌人
            List<Transform> enemiesInRange = new List<Transform>();
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (var enemy in enemies)
            {
                if (!enemy.gameObject.CompareTag(FriendTag) && enemy.gameObject != gameObject)
                {
                    enemiesInRange.Add(enemy.transform);
                }
            }

            // 如果范围内有敌人，攻击其中一个
            if (enemiesInRange.Count > 0)
            {
                AttackNearestEnemy(enemiesInRange[0]);
                nextAttackTime = Time.time + attackInterval; // 重置下次攻击时间
            }
        }
    }

    void AttackNearestEnemy(Transform enemy)
    {
        // 假设敌人也有这个脚本，直接调用敌人的TakeDamage方法
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
        //spumPrefabs.PlayAnimation(2);
        // 可以在这里添加额外的死亡逻辑，如播放动画、生成掉落物等。
    }
    bool IsIdle()                                  //判断静止
    {
        // 通过Rigidbody2D的velocity属性来判断角色是否静止
        return rb.velocity.sqrMagnitude <= 0;
    }
}
