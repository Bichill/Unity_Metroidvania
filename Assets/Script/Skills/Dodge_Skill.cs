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
        // CheckUnlocked() ���ڻ�� Start() ���Ԅ��{��
        
        if (unlockDodgeButton) unlockDodgeButton.GetComponent<Button>()?.onClick.AddListener(UnlockDodge);
        if (unlockMirageDodge) unlockMirageDodge.GetComponent<Button>()?.onClick.AddListener(UnlockMirageDodge);
    }


    private void UnlockDodge()
    {
        //���⼼���ظ��������ظ��������ֵ
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
        // �ȴ�һ�����_�� Inventory �� Start() �����ѽ�����
        yield return null;
        
        // �ٴΙz�� Inventory �Ƿ����
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