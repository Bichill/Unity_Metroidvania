using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck_1;
    [SerializeField] protected Transform groundCheck_2;
    [SerializeField] protected float groundCheckDistance_1;
    [SerializeField] protected float groundCheckDistance_2;
    [SerializeField] protected Transform wallCheck_1;
    [SerializeField] protected Transform wallCheck_2;
    [SerializeField] protected float wallCheckDistance_1;
    [SerializeField] protected float wallCheckDistance_2;
    [SerializeField] protected LayerMask whatIsGround;

    [Header("Friction info")]
    [SerializeField] private float originalFriction; // 记录初始摩擦力
    private PhysicsMaterial2D physicsMaterial; // 物理材质

    public int facingDir = 1;
    protected bool facingRight = true;

    [Header("Invincible info")]
    public bool isInvincible = false;

    public System.Action onFlipped;

    protected virtual void Awake() 
    {
    
    }

    protected virtual void Start() 
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFX>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();

        physicsMaterial = rb.sharedMaterial;
        if (physicsMaterial != null)
        {
            originalFriction = physicsMaterial.friction;
        }
    }

    protected virtual void Update() 
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    //判断是否接地
    public bool IsTouchingGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f, whatIsGround);
        return colliders.Length > 0;
    }


    //设置摩擦力
    public void SetFriction(float friction)
    {
        if (physicsMaterial != null)
        {
            physicsMaterial.friction = friction;
            rb.sharedMaterial = physicsMaterial;
        }
    }

    //恢复摩擦力
    public void RestoreOriginalFriction()
    {
        SetFriction(originalFriction);
    }

    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockback");
    }

    public virtual IEnumerator HitKnockback() 
    {
        isKnocked = true;
        rb.velocity = new Vector2(knockbackDirection.x * -facingDir, knockbackDirection.y);
        //等待击退时间
        yield return new WaitForSeconds(knockbackDuration);
        //确保完全落地后再重置状态
        /*
        while (!IsGroundDetected())
        {
            yield return null;
        }
        //确保完全落地后再重置状态
        yield return new WaitForSeconds(0.1f);
        */
        isKnocked = false;
    }

    #region Collison
    public virtual bool IsGroundDetected()
    {
        if (Physics2D.Raycast(groundCheck_1.position, Vector2.down, groundCheckDistance_1, whatIsGround) ||
            Physics2D.Raycast(groundCheck_2.position, Vector2.down, groundCheckDistance_2, whatIsGround))
        {
            return true;
        }
        return false;
    }
    public virtual bool IsWallDetected()
    {
        if (Physics2D.Raycast(wallCheck_1.position, Vector2.down, wallCheckDistance_1, whatIsGround) ||
            Physics2D.Raycast(wallCheck_2.position, Vector2.down, wallCheckDistance_2, whatIsGround))
        {
            return true;
        }
        return false;
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck_1.position, new Vector3(groundCheck_1.position.x,
            groundCheck_1.position.y - groundCheckDistance_1));
        Gizmos.DrawLine(groundCheck_2.position, new Vector3(groundCheck_2.position.x,
            groundCheck_2.position.y - groundCheckDistance_2));
        Gizmos.DrawLine(wallCheck_1.position, new Vector3(wallCheck_1.position.x + wallCheckDistance_1,
            wallCheck_1.position.y));
        Gizmos.DrawLine(wallCheck_2.position, new Vector3(wallCheck_2.position.x + wallCheckDistance_2,
            wallCheck_2.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
        Gizmos.color = Color.red;
    }
    #endregion

    #region Velocity
    public void SetZeroVelocity() 
    {
        if (isKnocked) return;
        rb.velocity = new Vector2(0, 0);
    }
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
        {
            return;
        }

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion
    
    #region Flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        //只有监听时才执行
        if (onFlipped != null)
        { 
            onFlipped();
        }
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion

    public virtual void Die()
    {
        fx.CancelColorChange();
        isInvincible = true;
    }

}
