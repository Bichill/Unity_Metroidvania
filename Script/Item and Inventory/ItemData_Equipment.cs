using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EquipmentType
{
    Weapon,//武器
    Armor,//护甲
    Amulet,//护符
    Flask,//药水
}

public enum EquipmentQuality
{
    evilSpirits,
    legend,
    epic,
    Sophisticated,
    excellent,
    ordinary
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;
    public EquipmentQuality equipmentQuality;

    public float itemCooldown;

    //装备的词条栏
    public ItemEffect[] itemEffects;

    [TextArea]
    public string effectDescription;

    [Header("Major stats")]
    public int strength; // 力量，每点+3攻击
    public int agility; // 敏捷，每点+1闪避
    public int intelligence; // 智力，每点+5法强
    public int vitality; // 体力，每点+10生命

    [Header("Offensive stats")]
    public int damage;
    public int critChance; // 暴击率
    public int critPower; // 暴击伤害

    [Header("Defensive stats")]
    public int health;
    public int armor; // 护甲，减少物理伤害
    public int evasion; // 闪避，概率免疫伤害
    public int magicResistence; // 魔法抗性，减少魔法伤害

    [Header("Magic Ignite")]
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    [Header("Craft requierments")]
    public List<InventoryItem> craftingMaterials; // 合成材料

    private int descriptionLength;

    public void Effect(Transform _enemyPosition)
    {
        if (itemEffects == null || itemEffects.Length == 0)
            return;
            
        foreach (var item in itemEffects)
        {
            if (item != null)
            {
                item.ExecuteEffect(_enemyPosition);
            }
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        // 記錄裝備前的血量百分比
        float healthPercentage = (float)playerStats.currentHealth / playerStats.CalculateMaxHealth();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistence.AddModifier(magicResistence);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamgae.AddModifier(lightingDamage);

        // 根據新的最大血量調整當前血量，保持百分比不變
        int newMaxHealth = playerStats.CalculateMaxHealth();
        playerStats.currentHealth = Mathf.RoundToInt(newMaxHealth * healthPercentage);
        
        // 觸發血量變化事件
        if (playerStats.onHealthChanged != null)
            playerStats.onHealthChanged();
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        // 記錄裝備前的血量百分比
        float healthPercentage = (float)playerStats.currentHealth / playerStats.CalculateMaxHealth();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistence.RemoveModifier(magicResistence);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamgae.RemoveModifier(lightingDamage);

        // 根據新的最大血量調整當前血量，保持百分比不變
        int newMaxHealth = playerStats.CalculateMaxHealth();
        playerStats.currentHealth = Mathf.RoundToInt(newMaxHealth * healthPercentage);
        
        // 觸發血量變化事件
        if (playerStats.onHealthChanged != null)
            playerStats.onHealthChanged();
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "力量"); 
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智力"); 
        AddItemDescription(vitality, "活力");

        AddItemDescription(damage, "物伤"); 
        AddItemDescription(critChance, "暴击概率");
        AddItemDescription(critPower, "暴击倍率");
        
        AddItemDescription(health, "血量"); 
        AddItemDescription(evasion, "闪避");
        AddItemDescription(armor, "护甲");
        AddItemDescription(magicResistence, "魔抗");

        AddItemDescription(fireDamage, "火焰伤害"); 
        AddItemDescription(iceDamage, "冰冻伤害");
        AddItemDescription(lightingDamage, "雷电伤害");

        if (descriptionLength < 5)
        {
            for (int i = 0; i < 5-descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append(" ");
            }
        }

        return sb.ToString();
    }   

    private void AddItemDescription(int _value ,string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            if (_value > 0)
                sb.Append(" + " + _name + "  " + _value);

            descriptionLength++;
        }
    }
}
