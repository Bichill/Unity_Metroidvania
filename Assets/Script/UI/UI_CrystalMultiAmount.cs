using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class UI_CrystalMultiAmount : MonoBehaviour
{
    private TextMeshProUGUI amount;
    private Transform child;
    private Transform playerTransform;
    private Vector3 offsetFromPlayer;
    private Vector3 originalLocalScale;

    void Start()
    {
        amount = GetComponentInChildren<TextMeshProUGUI>();
        child = transform.GetChild(0);
        
        // 保存原始縮放
        originalLocalScale = transform.localScale;

        playerTransform = transform.parent;
        // 計算相對於玩家的偏移量
        offsetFromPlayer = transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // 檢查 SkillManager 和 crystal 是否為空
        if (SkillManager.instance == null || SkillManager.instance.crystal == null)
        {
            Debug.LogWarning("SkillManager.instance 或 crystal 為空");
            return;
        }

        // 跟隨玩家位置但不跟隨旋轉和縮放
        if (playerTransform != null)
        {
            // 如果是玩家的子對象，使用相對偏移
            transform.position = playerTransform.position + offsetFromPlayer;

            // 強制保持UI的原始旋轉，不受玩家旋轉影響
            transform.rotation = Quaternion.identity;
            // 強制保持UI的原始縮放，防止對稱翻轉
            transform.localScale = originalLocalScale;
        }

        // 檢查多重水晶是否解鎖
        if (SkillManager.instance.crystal.crystalMultiUnlocked)
        {
            child.gameObject.SetActive(true);
            amount.text = "×" + SkillManager.instance.crystal.GetCurrentCrystalLeftAmount().ToString();
        }
        else
        {
            child.gameObject.SetActive(false);  
        }
    }
}
