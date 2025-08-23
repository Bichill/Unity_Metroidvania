using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EquipmentType
{
    Weapon,//����
    Armor,//����
    Amulet,//����
    Flask,//ҩˮ
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

    //װ���Ĵ�����
    public ItemEffect[] itemEffects;

    [TextArea]
    public string effectDescription;

    [Header("Major stats")]
    public int strength; // ������ÿ��+3����
    public int agility; // ���ݣ�ÿ��+1����
    public int intelligence; // ������ÿ��+5��ǿ
    public int vitality; // ������ÿ��+10����

    [Header("Offensive stats")]
    public int damage;
    public int critChance; // ������
    public int critPower; // �����˺�

    [Header("Defensive stats")]
    public int health;
    public int armor; // ���ף����������˺�
    public int evasion; // ���ܣ����������˺�
    public int magicResistence; // ħ�����ԣ�����ħ���˺�

    [Header("Magic Ignite")]
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    [Header("Craft requierments")]
    public List<InventoryItem> craftingMaterials; // �ϳɲ���

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

        // ӛ��b��ǰ��Ѫ���ٷֱ�
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

        // �����µ����Ѫ���{����ǰѪ�������ְٷֱȲ�׃
        int newMaxHealth = playerStats.CalculateMaxHealth();
        playerStats.currentHealth = Mathf.RoundToInt(newMaxHealth * healthPercentage);
        
        // �|�lѪ��׃���¼�
        if (playerStats.onHealthChanged != null)
            playerStats.onHealthChanged();
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        // ӛ��b��ǰ��Ѫ���ٷֱ�
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

        // �����µ����Ѫ���{����ǰѪ�������ְٷֱȲ�׃
        int newMaxHealth = playerStats.CalculateMaxHealth();
        playerStats.currentHealth = Mathf.RoundToInt(newMaxHealth * healthPercentage);
        
        // �|�lѪ��׃���¼�
        if (playerStats.onHealthChanged != null)
            playerStats.onHealthChanged();
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "����"); 
        AddItemDescription(agility, "����");
        AddItemDescription(intelligence, "����"); 
        AddItemDescription(vitality, "����");

        AddItemDescription(damage, "����"); 
        AddItemDescription(critChance, "��������");
        AddItemDescription(critPower, "��������");
        
        AddItemDescription(health, "Ѫ��"); 
        AddItemDescription(evasion, "����");
        AddItemDescription(armor, "����");
        AddItemDescription(magicResistence, "ħ��");

        AddItemDescription(fireDamage, "�����˺�"); 
        AddItemDescription(iceDamage, "�����˺�");
        AddItemDescription(lightingDamage, "�׵��˺�");

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
