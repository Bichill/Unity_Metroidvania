using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CapsuleCollider2D cd;

    private bool canRotate ;
    private bool isReturning = false;

    private float freezeTimeDuration;
    private float vulnerableDuration;
    private float returnSpeed;
    private float maxSwordReturnDistance;

    [Header("Pierce info")]
    private float pierceAmount;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;
    private Vector2 spinDirection;
    private float spinCollsionCount = 1;//锯旋剑碰撞一次后就开始读条持续时间

    private float hitTimer;
    private float hitCooldown;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim.SetBool("Rotation", true); 
    }

    public void SetupSword(Vector2 _dir,float _gravityScale,float _freezeTimeDuration,float _vulnerableDuration, float _returnSpeed,
        float _maxSwordReturnDistance)
    {
        AudioManager.instance.PlaySFX(10, transform, 1, 1f);
        rb.velocity =  _dir;
        rb.gravityScale = _gravityScale;
        freezeTimeDuration = _freezeTimeDuration;
        vulnerableDuration = _vulnerableDuration;
        returnSpeed = _returnSpeed;
        maxSwordReturnDistance = _maxSwordReturnDistance;
        spinDirection = new Vector2(Mathf.Clamp(rb.velocity.x, -1, 1), Mathf.Clamp(rb.velocity.y, -.1f, .1f));
        // 根据速度方向设置旋转角度
        if (_dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void SetupBounce(bool _isBounceing,int _amountOfBounces,float _bounceSpeed)
    {
        isBouncing= _isBounceing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;
        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration ,float _hitCooldown)
    {
        AudioManager.instance.PlaySFX(18, transform, 1f, 1f);
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll; 
        //rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;


    }

    private void Update()
    {
        if (canRotate)
        {
            transform.right = rb.velocity;
        }

        if (isReturning)
        {
            anim.SetBool("Rotation", true);
            transform.position = Vector2.MoveTowards(transform.position, PlayerManager.instance.player.transform.position,
                returnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position) < 0.7)
            {
                PlayerManager.instance.player.CatchTheSword();
            }
        }
        //投掷物超过距离销毁
        if (Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position) > maxSwordReturnDistance)
        {
            DestroyMe();
        }
        BounceLogic();
        SpinLogic();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(PlayerManager.instance.player.transform.position, transform.position)
                > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }
            }

            hitTimer -= Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position,new Vector2(transform.position.x + spinDirection.x, 
                transform.position.y + spinDirection.y), 1.5f * Time.deltaTime);

            if (hitTimer < 0)
            {
                hitTimer = hitCooldown;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.2f);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }
            }
        }
    }

    //停止旋转剑
    private void StopWhenSpinning()
    {
        AudioManager.instance.StopSFX(18);
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            anim.SetBool("Rotation", true);
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position,
                bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                {
                    targetIndex = 0;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning) return;

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }

        if (isBouncing) SetupTargetsForBounce(collision);
        StuckInto(collision);
    }

    //剑触发敌人damage与僵直效果
    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();


        // 如果解锁了僵直效果技能，则对敌人造成僵直效果
        if (PlayerManager.instance.player.skill.sword.timeStopUnlocked)
            enemy.StartCoroutine(enemy.FreezeTimerForCoroutine(freezeTimeDuration));
        // 如果解锁了脆弱效果技能，则对敌人造成脆弱效果
        if (PlayerManager.instance.player.skill.sword.vulnerableUnlocked)
            enemyStats.MakeVulnerable(vulnerableDuration);

        PlayerManager.instance.player.stats.DoDamage(enemyStats);

        ItemData_Equipment equipmentAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

        if (equipmentAmulet != null)
        {
            equipmentAmulet.Effect(enemy.transform);
        }
    }

    //设置弹跳剑的目标列表
    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {

            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            if (spinCollsionCount > 0)
            {
                spinCollsionCount--;
                StopWhenSpinning();
            }
            return;  
        }

        anim.SetBool("Rotation", false);
        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
        {
            return;
        }

        transform.parent = collision.transform;
    }
}
