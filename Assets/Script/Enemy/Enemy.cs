using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;
    private Coroutine slowCoroutine;// 冰缓协程，防止减速效果叠加
    private bool isFrozen = false; // 是否被黑洞冻结，防止冰缓解除黑洞定身

    [Header("Stunned info")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] public GameObject counterImage;
    [SerializeField] public GameObject stunnedFx;

    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;

    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    [HideInInspector] public float lastAttackTime;

    
    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName {  get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defaultMoveSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public virtual void AssignLastAnimName(string _lastBoolName) => lastAnimBoolName = _lastBoolName;

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        // 每次冰缓前先恢复默认速度
        moveSpeed = defaultMoveSpeed;
        anim.speed = 1f;

        moveSpeed = moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        // 如果已有减速协程，先停止
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowRecover(_slowDuration));
    }

    private IEnumerator SlowRecover(float duration)
    {
        yield return new WaitForSeconds(duration);
        ReturnDefaultSpeed();
        slowCoroutine = null;
    }

    // 恢复默认速度
    protected override void ReturnDefaultSpeed()
    {
        // 如果是被黑洞冻结的敌人，不恢复速度
        if (!isFrozen)
        {
            base.ReturnDefaultSpeed();

            moveSpeed = defaultMoveSpeed;
        }
    }

    //黑洞定身
    public virtual void FreezeTimer(bool _timerFrozen)
    {
        if (_timerFrozen)
        {
            isFrozen = true;
            moveSpeed = 0f;
            anim.speed = 0f;
        }
        else
        {
            isFrozen = false;
            ReturnDefaultSpeed();
            anim.speed = 1f;
        }
    }

    public virtual IEnumerator FreezeTimerForCoroutine(float _seconds)
    {
        FreezeTimer(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTimer(false);
    }

    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }

    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected()
    {
        RaycastHit2D playerHit = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 7, whatIsPlayer);
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir , 7, whatIsGround);

        if (!playerHit || (wallHit && wallHit.distance < playerHit.distance))
        {
            return new RaycastHit2D();
        }
        return playerHit;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,
            new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }

}
