using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWindbreaker_Controller : MonoBehaviour
{
	private int damage;
	[SerializeField] private float attackLongth; // 攻擊範圍長
	[SerializeField] private float attackWidth; // 攻擊範圍寬
	[SerializeField] private float attackMultiplier; // 攻擊傷害倍率

	private void Start()
	{
        AttackAllEnemiesInRange();

		Destroy(gameObject, 1f);
	}

	public void Setup(int _damage)
	{
		damage = _damage;
	}

	private void AttackAllEnemiesInRange()
	{
		// 使用矩形盒碰撞檢測長方形範圍內的所有碰撞體
		Vector2 center = transform.position;
		Vector2 size = new Vector2(attackLongth, attackWidth);
		Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0f);

		int finalDamage = Mathf.RoundToInt(damage * attackMultiplier);

		foreach (var collider in colliders)
		{
			Enemy enemy = collider.GetComponent<Enemy>();
			if (enemy != null && !enemy.isInvincible)
			{
				CharacterStats target = enemy.GetComponent<CharacterStats>();
				if (target != null && !target.isDead)
				{
					target.TakeDamage(finalDamage);
				}
			}
		}
	}

	// 在 Gizmos 中顯示攻擊範圍（用於調試）
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(attackLongth, attackWidth, 0f));
	}
}
