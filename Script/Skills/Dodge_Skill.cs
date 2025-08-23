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
        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodge.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }


    private void UnlockDodge()
    {
        //避免技能重复解锁，重复添加闪避值
        if (isDodgeUnlocked) return;

        if (unlockDodgeButton.unlocked)
        {
            isDodgeUnlocked = true;

            player.stats.evasion.AddModifier(amountEvasionAdded);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }
    }

    private void UnlockMirageDodge()
    {
        if(unlockMirageDodge.unlocked)
            dodgcMirageUnlocked = true;
    }

    public void CreatMirageOnDodge()
    {
        if(dodgcMirageUnlocked)
            SkillManager.instance.clone.CreatClone(FindClosestEnemy(player.transform), Vector3.zero);
    }
}