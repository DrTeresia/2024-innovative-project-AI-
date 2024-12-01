using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class character : MonoBehaviour
{
    [Header("属性")]

    [SerializeField]protected float maxHealth;  //最大血量
    [SerializeField]protected float currentHealth; //当前血量
    // Start is called before the first frame update
    [Header("无敌")]
    public bool invulnerable;
    public float invulnerableDuration;  //无敌时间

    [Header("血条")]
    public Image healthBarFill; // 当前血量的Image组件
    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }


    protected virtual void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            // 计算血量比例
            float healthRatio = currentHealth / maxHealth;
            // 更新血条填充的宽度或高度
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
        UpdateHealthBar(); // 更新血条显示

        StartCoroutine(nameof(InvulnerableCoroutine));  //启动无敌时间携程

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        currentHealth = 0f;
        Destroy(this.gameObject);  //销毁该对象
    }

    //无敌
    protected virtual IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerableDuration); //无敌时间
        invulnerable = false;
    }

}
