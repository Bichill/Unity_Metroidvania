using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackHole_Skill : Skill
{
    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackHoleUnlocked { get; private set; }
    [SerializeField] private int amountOfAttack;
    [SerializeField] private float cloneCooldown;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float allowAttackDuration;


    public override bool CanUseSkill()
    {   
        if (!blackHoleUnlocked) return false;
        Debug.Log("BlackHole Skill Locked");
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab,player.transform.position,Quaternion.identity);

        Blackhole_Skill_Controller newBlackHoleScript = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        newBlackHoleScript.SetupBlackHole(maxSize,growSpeed,shrinkSpeed,amountOfAttack,cloneCooldown,allowAttackDuration);
    }

    protected override void Start()
    {
        base.Start();
        // CheckUnlocked() 會在基類 Start() 中自動調用
        blackHoleUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockBlackHole);
    }

    protected override void Update()
    {
        base.Update();
    }

    private void UnlockBlackHole()
    {
        if (blackHoleUnlockButton.unlocked)
        {
            blackHoleUnlocked = true;
        }
    }

    protected override void CheckUnlocked()
    {
        UnlockBlackHole();
    }


}
