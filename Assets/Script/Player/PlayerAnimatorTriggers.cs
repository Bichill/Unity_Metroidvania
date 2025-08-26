using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<Enemy>().isInvincible)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                player.stats.DoDamage(_target);

                // ôz≤È «∑Ò”–—bÇ‰Œ‰∆˜£¨±‹√‚ null reference exception
                ItemData_Equipment weapon = Inventory.instance.GetEquipment(EquipmentType.Weapon);
                
                if (weapon != null)
                {
                    weapon.Effect(_target.transform);
                }
            }
        }
    }

    private void ThordSword()
    {
        if (SkillManager.instance.sword.CanUseSkill())
        {
            SkillManager.instance.sword.CreatSword();
        }
    }
}
