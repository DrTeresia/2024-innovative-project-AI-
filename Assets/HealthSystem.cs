using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float vitality = 1.0f; // ������������Ӱ��ָ����������ֵ
    [SerializeField] private string teamTag = "TeamA"; // �Լ�����Ӫ��ǩ
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1.0f;

    private Animator anim;
    private bool isDead = false;
    private float attackTimer = 0f;
    private Coroutine attackCoroutine;
    public float MaxHealth => maxHealth;  
    public float CurrentHealth => currentHealth;

    private Castle castle;

    void Start()
    {
        castle = GetComponent<Castle>();
        string currentLayerName = LayerMask.LayerToName(gameObject.layer);
        if (currentLayerName == "General")
        {
            damage = 40f;
        }
        else
        {
            damage = 15f;
        }
        currentHealth = maxHealth;
        teamTag = gameObject.tag;
        anim = GetComponent<Animator>();
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (!isDead)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (!isDead)
        {
            Collider2D[] hitUnits = Physics2D.OverlapCircleAll(transform.position, attackRange);

            foreach (Collider2D unit in hitUnits)
            {
                if (unit.CompareTag(teamTag)) continue; // ����ͬ��Ӫ��λ

                HealthSystem targetHealth = unit.GetComponent<HealthSystem>();
                if (targetHealth != null && attackTimer <= 0)
                {
                    targetHealth.TakeDamage(damage);
                    attackTimer = attackCooldown;

                    // ���Ź�������
                    if (anim != null)
                    {
                        anim.SetTrigger("Attack");
                    }
                }
            }

            yield return new WaitForSeconds(0.5f); // �������Ƶ��
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // �������˶���
        if (anim != null)
        {
            anim.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // ֹͣ����Э��
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }


        // �ӳ����ٶ�����ȷ�������������
        string LayerName = LayerMask.LayerToName(gameObject.layer);
        if (LayerName != "City")
        {
            Invoke("DestroyObject", 0f);
        }
        else {
            castle.spawnCount = 0;
            castle.tag = "destoried";
        }
       
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }


    // ͨ�������¼����ô˷���
    public void AttackTrigger()
    {
        // ����Ĺ����߼�
    }
}