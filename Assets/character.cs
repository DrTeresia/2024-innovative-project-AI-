using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class character : MonoBehaviour
{
    [Header("����")]

    [SerializeField]protected float maxHealth;  //���Ѫ��
    [SerializeField]protected float currentHealth; //��ǰѪ��
    // Start is called before the first frame update
    [Header("�޵�")]
    public bool invulnerable;
    public float invulnerableDuration;  //�޵�ʱ��

    [Header("Ѫ��")]
    public Image healthBarFill; // ��ǰѪ����Image���
    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }


    protected virtual void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            // ����Ѫ������
            float healthRatio = currentHealth / maxHealth;
            // ����Ѫ�����Ŀ�Ȼ�߶�
            healthBarFill.rectTransform.sizeDelta = new Vector2(healthBarFill.rectTransform.sizeDelta.x * healthRatio, healthBarFill.rectTransform.sizeDelta.y);
        }
    }
    public virtual void TakeDamage(float damage)
    {
        if (invulnerable)
        {
            return;
        }
        currentHealth -= damage;
        UpdateHealthBar(); // ����Ѫ����ʾ

        StartCoroutine(nameof(InvulnerableCoroutine));  //�����޵�ʱ��Я��

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        currentHealth = 0f;
        Destroy(this.gameObject);  //���ٸö���
    }

    //�޵�
    protected virtual IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerableDuration); //�޵�ʱ��
        invulnerable = false;
    }

}
