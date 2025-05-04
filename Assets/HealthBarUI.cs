using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Image backgroundImage;

    private void Start()
    {
        healthSystem = GetComponentInParent<HealthSystem>();
        UpdateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthSystem == null) return;
        float healthPercent = healthSystem.CurrentHealth / healthSystem.MaxHealth;
        //foregroundImage.fillAmount = healthPercent;
        if (Mathf.Approximately(healthPercent, 1f)) // 满血时
        {
            SetAlpha(0f); // 完全透明
        }
        else // 受伤时
        {
            SetAlpha(1f); // 完全可见
            foregroundImage.fillAmount = healthPercent; // 更新血条长度
        }
    }
    // 设置血条透明度
    private void SetAlpha(float alpha)
    {
        Color tempColor = foregroundImage.color;
        Color tempColor2 = backgroundImage.color;
        tempColor.a = alpha;
        tempColor2.a = alpha;
        foregroundImage.color = tempColor;
        backgroundImage.color = tempColor2;
    }
}