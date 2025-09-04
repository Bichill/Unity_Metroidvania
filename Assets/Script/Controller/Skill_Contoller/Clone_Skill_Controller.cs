using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;
    
    private float cloneTimer;
    [SerializeField] private float attackMultiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir = 1 ;

    private float chanceToDuplicate;
    private bool canDuplicateClone;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;
        
        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1 , sr.color.a - (Time.deltaTime * colorLoosingSpeed));
        }
        if (sr.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetupClone(Transform _newTransform, float _cloneDuration , Vector3 _offset,
        Transform _closestEnemy,bool _canDuplicateClone,float _chanceToDupliacte,float _attackMultiplier)
    {
        anim.SetInteger("AttackNumber", Random.Range(1, 4));
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        sr.color = new Color(1, 1, 1, 0.3f);
        
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicateClone; 
        chanceToDuplicate = _chanceToDupliacte;
        attackMultiplier = _attackMultiplier;

        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        AudioManager.instance.PlaySFX(5, transform, Random.Range(1.1f, 1.4f), 0.3f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<Enemy>().isInvincible)
            {
                PlayerManager.instance.player.stats.DoDamage(hit.GetComponent<CharacterStats>());

                if (PlayerManager.instance.player.skill.clone.unlockAggresiveClone)
                {
                    // ôz≤È «∑Ò”–—bÇ‰Œ‰∆˜£¨±‹√‚ null reference exception
                    ItemData_Equipment weapon = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weapon != null)
                    {
                        weapon.Effect(hit.transform);
                    }
                }

                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreatClone(hit.transform, new Vector3(1.3f * facingDir, 0));
                    }
                }

            }
        }
    }

    private void FaceClosestTarget()
    {
        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir *= -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
