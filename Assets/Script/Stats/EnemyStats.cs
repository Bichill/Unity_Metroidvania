using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    [SerializeField] private Stats skillpointDropAmount = new Stats();

    [Header("Level details")]
    [SerializeField] private int level;

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = 0.3f;//每级增强30%百分比

    protected override void Start()
    {
        skillpointDropAmount.SetDefaultValue(30);
        // 先應用等級修改器，再初始化血量
        ApplyLevelModifiers();
        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    // 暴击率、暴击倍率、闪避不随等级增加而增加
    private void ApplyLevelModifiers()
    {
        Modify(strength); 
        Modify(agility); 
        Modify(intelligence); 
        Modify(vitality);

        Modify(damage); 
        //Modify(critChance); 
        //Modify(critPower);
        
        Modify(maxHealth); 
        Modify(armor); 
        //Modify(evasion); 
        Modify(magicResistence);
        
        Modify(fireDamage); 
        Modify(iceDamage); 
        Modify(lightingDamgae);

        Modify(skillpointDropAmount);
    }

    private void Modify(Stats _stats)
    {
        int value = _stats.GetValue();
        
        for (int i = 1; i <= level; i++)
        {
            float modifier = value * percentageModifier * i;
            _stats.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        AudioManager.instance.PlaySFX(17, transform, Random.Range(1.1f, 1.3f), 0.5f);
    }

    protected override void Die()
    {
        if(isDead)return;
        ChangeDeadToTrue();//将isDead改为true

        base.Die();
        enemy.Die();

        PlayerManager.instance.currency += skillpointDropAmount.GetValue();
        myDropSystem.GenerateDrop(); // 掉落物品
    }
}
