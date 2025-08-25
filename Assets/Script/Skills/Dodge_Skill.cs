using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    public bool dodgeUnlocked;
    public int amountEvasionAdded;
    private bool isDodgeUnlocked = false;

    [Header("Mirage dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodge;
    public bool dodgcMirageUnlocked;

    protected override void Start()
    {
        base.Start();
        // CheckUnlocked() 會在基類 Start() 中自動調用
        
        if (unlockDodgeButton) unlockDodgeButton.GetComponent<Button>()?.onClick.AddListener(UnlockDodge);
        if (unlockMirageDodge) unlockMirageDodge.GetComponent<Button>()?.onClick.AddListener(UnlockMirageDodge);
    }


    private void UnlockDodge()
    {
        //避免技能重复解锁，重复添加闪避值
        if (isDodgeUnlocked) return;

        if (unlockDodgeButton.unlocked)
        {
            isDodgeUnlocked = true;

            if (player != null && player.stats != null)
            {
                player.stats.evasion.AddModifier(amountEvasionAdded);
            }
            
            dodgeUnlocked = true;
        }
    }

    private IEnumerator DelayedUpdateStatsUI()
    {
        // 等待一幀，確保 Inventory 的 Start() 方法已經執行
        yield return null;
        
        // 再次檢查 Inventory 是否可用
        if (Inventory.instance != null)
        {
            Inventory.instance.UpdateStatsUI();
        }
    }

    private void UnlockMirageDodge()
    {
        if(unlockMirageDodge.unlocked)
            dodgcMirageUnlocked = true;
    }

    protected override void CheckUnlocked()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    public void CreatMirageOnDodge()
    {
        if(dodgcMirageUnlocked && SkillManager.instance != null && SkillManager.instance.clone != null)
        {
            SkillManager.instance.clone.CreatClone(FindClosestEnemy(player.transform), Vector3.zero);
        }
    }
}