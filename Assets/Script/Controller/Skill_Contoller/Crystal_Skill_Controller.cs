using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExitTimer;
    private bool canExplode;
    private bool canMove;
    private float moveSpeed;
    private float searchRadius;
    private bool canGrow;
    private float growSpeeed = 5f;
    private Transform RandomTarget;
    [SerializeField] private LayerMask whatIsEnemy;
    
    public void ChooseRandomTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius, whatIsEnemy);
        if (colliders.Length > 0)
        {
            RandomTarget = colliders[Random.Range(0,colliders.Length)].transform;
        }
    }

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, float _searchRadius)
    {
        crystalExitTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        searchRadius = _searchRadius;
    }

    private void Update()
    {
        crystalExitTimer -= Time.deltaTime;
        if (crystalExitTimer < 0)
        {
            CrystalDestroyLogic();
        }

        if (canMove)
        {
            if (RandomTarget != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, RandomTarget.position, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, RandomTarget.position) < 0.7)
                {
                    CrystalDestroyLogic();
                }
            }
        }

        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector3(3, 3), growSpeeed * Time.deltaTime);
        }
    }

    public void CrystalDestroyLogic()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {   
            selfDestroy();
        }
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<Enemy>().isInvincible)
            {
                PlayerManager.instance.player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());

                ItemData_Equipment equipmentAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

                if (equipmentAmulet != null)
                {
                    equipmentAmulet.Effect(hit.transform);
                }
            }
        }
    }

    public void selfDestroy() => Destroy(gameObject);
}
