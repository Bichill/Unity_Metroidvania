using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    private bool isApplyingArmorEffect = false;

    [Header("GhostlyWindbreaker")]
    [SerializeField] private GameObject ghostPrefab;


    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        player.Die();
        GetComponent<PlayerItemDrop>().GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (isApplyingArmorEffect)
            return;

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
        {
            isApplyingArmorEffect = true;
            currentArmor.Effect(null);
        }
    }

    public override void OnEvasion()
    {
        Debug.Log("Player evaded the attack!");
        SkillManager.instance.dodge.CreatMirageOnDodge();

        if(IsGhostlyWindbreakerEquipped())
            UseGhostWindbreakerEffect();
    }

    // 检查是否装备了幽灵风衣
    private bool IsGhostlyWindbreakerEquipped()
    {
        ItemData_Equipment armor = Inventory.instance.GetEquipment(EquipmentType.Armor);
        if (armor == null || armor.itemEffects == null)
            return false;

        foreach (var effect in armor.itemEffects)
        {
            if (effect is GhostlyWindbreaker_Effect)
                return true;
        }
        return false;
    }

    private void UseGhostWindbreakerEffect()
    {
        GameObject newBladeLight = Instantiate(ghostPrefab, player.transform.position, Quaternion.identity);

        // 設置刀光朝向
        newBladeLight.transform.right = player.facingDir * Vector3.right;

        newBladeLight.GetComponent<GhostWindbreaker_Controller>().Setup(player.stats.damage.GetValue());
    }
}
