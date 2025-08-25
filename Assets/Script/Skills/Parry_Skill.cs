using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    public bool restoreUnlocked { get; private set; }

    [Header("Parry with mirage")]
    [SerializeField] private UI_SkillTreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();
        // CheckUnlocked() 會在基類 Start() 中自動調用

        parryUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockRestore);
        parryWithMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockParryWithMirage);
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int restoreHealth = Mathf.RoundToInt(player.stats.CalculateMaxHealth() * Random.Range(.1f, .15f));
            player.stats.IncreaseHealthBy(restoreHealth);
        }
    }

    public void UnlockParry()
    {
        if(parryUnlockButton.unlocked)
            parryUnlocked = true;
    }

    public void UnlockRestore()
    {
        if(restoreUnlockButton.unlocked)
            restoreUnlocked = true;
    }

    public void UnlockParryWithMirage()
    {
        if(parryWithMirageUnlockButton.unlocked)
            parryWithMirageUnlocked = true;
    }

    protected override void CheckUnlocked()
    {
        UnlockParry();
        UnlockRestore();
        UnlockParryWithMirage();
    }

    public void CreatMirageOnParry(Transform _transformToSpawn)
    {
        if (parryWithMirageUnlocked && SkillManager.instance != null && SkillManager.instance.clone != null)
        {
            SkillManager.instance.clone.CreateCloneWithDelay(_transformToSpawn);
        }
    }


}
