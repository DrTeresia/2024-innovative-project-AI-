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
        if (Mathf.Approximately(healthPercent, 1f)) // ��Ѫʱ
        {
            SetAlpha(0f); // ��ȫ͸��
        }
        else // ����ʱ
        {
            SetAlpha(1f); // ��ȫ�ɼ�
            foregroundImage.fillAmount = healthPercent; // ����Ѫ������
        }
    }
    // ����Ѫ��͸����
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