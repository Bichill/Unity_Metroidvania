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
    [SerializeField] private float attackRadius = 2f; // ��������
    [SerializeField] private float attackMultiplier; // �����������

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
        
        // ������s�Y���ṥ�������ȵ����Д���
        if (hitTimer <= 0)
        {
            hitTimer = hitCooldown;
            AttackAllEnemiesInRange();
        }
    }

    private void AttackAllEnemiesInRange()
    {
        // �z�y�����ȵ�������ײ�w
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        damage = (int)(damage * attackMultiplier); // ���ù�������

        foreach (var collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && !enemy.isInvincible && !enemy.GetComponent<CharacterStats>().isDead)
            {
                CharacterStats target = enemy.GetComponent<CharacterStats>();

                // ��ɂ���
                target.TakeDamage(damage);

                if (randomNum == 0)
                {
                    // �����Ե��⣺��ɱ���Ч��
                    target.ApplyAilments(false, true, false, target);
                }
                else if (randomNum == 1)
                {
                    // �����Ե��⣺����cȼЧ��
                    float igniteMultiplier = Random.Range(0.15f, 0.25f);
                    int igniteDps = (int)(Mathf.Max(damage * igniteMultiplier, 1));
                    target.ApplyIgnite(target.ailmentsDuration, igniteDps);
                    
                    // �@ʾ�Ɵ� FX Ч��
                    target.GetComponent<EntityFX>()?.IgniteFxFor(target.ailmentsDuration);
                }
            }
        }
    }

    // ���x���� Gizmos ���@ʾ��������������{ԇ��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
