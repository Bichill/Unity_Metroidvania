using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWindbreaker_Controller : MonoBehaviour
{
	private int damage;
	[SerializeField] private float attackLongth; // ���������L
	[SerializeField] private float attackWidth; // ����������
	[SerializeField] private float attackMultiplier; // �����������

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
		// ʹ�þ��κ���ײ�z�y�L���ι����ȵ�������ײ�w
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

	// �� Gizmos ���@ʾ��������������{ԇ��
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(attackLongth, attackWidth, 0f));
	}
}
