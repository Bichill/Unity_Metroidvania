using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();

        entity.onFlipped += FilpUI;
        myStats.onHealthChanged += UpdateHealthUI;
        
        // ��ʼ���r��������UI
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (myStats == null || slider == null) return;
        
        if (myStats.currentHealth <= 0) 
        {
            gameObject.SetActive(false);
            return;
        }
        
        int maxHealth = myStats.CalculateMaxHealth();
        if (maxHealth <= 0) return; // ��ֹ������
        
        slider.maxValue = maxHealth;
        slider.value = myStats.currentHealth;
        
        // �_��Ѫ���������^���ֵ
        if (myStats.currentHealth > maxHealth)
        {
            myStats.currentHealth = maxHealth;
        }
    }

    private void FilpUI() => myTransform.Rotate(0f, 180f, 0f);
    
    private void OnDisable()
    {
        if (entity != null)
            entity.onFlipped -= FilpUI;
        if (myStats != null)
            myStats.onHealthChanged -= UpdateHealthUI;
    }
}
