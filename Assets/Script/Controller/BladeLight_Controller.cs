using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeLight_Controller: MonoBehaviour
{
    private Animator anim;
    private int damage;
    private int randomNum;
    private float hitCooldown = 0.3f;
    private float hitTimer;
    [SerializeField] private float attackRadius = 2f; // 攻擊範圍
    [SerializeField] private float attackMultiplier; // 攻擊傷害倍率

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetInteger("randomNum", randomNum);
        }
        Destroy(gameObject, 2f);
    }

    public void Setup(int _damage, int _randomNum)
    {
        damage = _damage;
        randomNum = _randomNum;
    }

    private void Update()
    {
        hitTimer -= Time.deltaTime;
        
        // 攻擊冷卻結束後攻擊範圍內的所有敵人
        if (hitTimer <= 0)
        {
            hitTimer = hitCooldown;
            AttackAllEnemiesInRange();
        }
    }

    private void AttackAllEnemiesInRange()
    {
        // 檢測範圍內的所有碰撞體
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        damage = (int)(damage * attackMultiplier); // 應用攻擊倍率

        foreach (var collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && !enemy.isInvincible && !enemy.GetComponent<CharacterStats>().isDead)
            {
                CharacterStats target = enemy.GetComponent<CharacterStats>();

                // 造成傷害
                target.TakeDamage(damage);

                if (randomNum == 0)
                {
                    // 冰屬性刀光：造成冰凍效果
                    target.ApplyAilments(false, true, false, target);
                }
                else if (randomNum == 1)
                {
                    // 火屬性刀光：造成點燃效果
                    float igniteMultiplier = Random.Range(0.15f, 0.25f);
                    int igniteDps = (int)(Mathf.Max(damage * igniteMultiplier, 1));
                    target.ApplyIgnite(target.ailmentsDuration, igniteDps);
                    
                    // 顯示灼燒 FX 效果
                    target.GetComponent<EntityFX>()?.IgniteFxFor(target.ailmentsDuration);
                }
            }
        }
    }

    // 可選：在 Gizmos 中顯示攻擊範圍（用於調試）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
