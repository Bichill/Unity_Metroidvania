using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingEnemies_Controller : MonoBehaviour
{
    private Animator anim;
    private int damage;
    private int freezingDuration;
    [SerializeField] private float attackRadius; // 攻擊範圍

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("freezing");
        }
        FreezingAllEnemiesInRange();
        Destroy(gameObject, 2f);
    }

    public void Setup(int _damage,int _duration)
    {
        damage = _damage;
        freezingDuration = _duration;
    }

    private void FreezingAllEnemiesInRange()
    {
        AudioManager.instance.PlaySFX(2, transform, 1.5f, 0.3f);

        // 檢測範圍內的所有碰撞體
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        foreach (var collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && !enemy.isInvincible && !enemy.GetComponent<CharacterStats>().isDead)
            {
                CharacterStats target = enemy.GetComponent<CharacterStats>();

                // 造成傷害
                target.TakeDamage(damage);
                // 造成冰凍效果
                target.ApplyAilments(false, true, false, target);
                enemy.StartCoroutine(enemy.FreezeTimerForCoroutine(freezingDuration));
            }
        }
    }

    // 可選：在 Gizmos 中顯示攻擊範圍（用於調試）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
