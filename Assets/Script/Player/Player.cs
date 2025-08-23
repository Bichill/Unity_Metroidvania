using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    public bool isBusy { get; private set; }
    [Header("Move info")]
    public float moveSpeed;
    public float jumpForce;
    public int jumpCount;
    public float swordReturnImpact;
    public float defaultMoveSpeed;
    public float defaultJumpForce;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    [SerializeField] public float dashOverGravity;
    public float defaulDashSpeed;

    [Header("Friction info")]
    private bool isFrictionZero = false;


    public float dashDir { get; private set; }

    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }

    #region State
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }

    public PlayerAirState airState { get; private set; }

    public PlayerDashState dashState { get; private set; }

    public PlayerWallslidState wallSlideState { get; private set; }

    public PlayerWallJumpState WallJumpState { get; private set; }
    //״̬���state�����ܣ���״̬���󲻼�state
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimSword { get; private set; }    
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackholeState blackHole { get; private set; }
    public PlayerDeadState dead { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallslidState(this, stateMachine, "WallSlide");
        WallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackholeState(this, stateMachine, "Jump");
        dead = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);//��̬������ת��

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaulDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
        CheckFrictionCondition();
            
        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.CanUseSkill();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Inventory.instance.UseFlask();
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaulDashSpeed;
    }

    private void CheckFrictionCondition()
    {
        if (isKnocked) return;

        bool isInJumpOrAirState = (stateMachine.currentState is PlayerJumpState ||
            stateMachine.currentState is PlayerAirState);
        bool isWallAndGroundNotDetected = (!IsWallDetected() && !IsGroundDetected());

        if (isInJumpOrAirState && isWallAndGroundNotDetected)
        {
            if (!isFrictionZero)
            {
                SetFriction(0);
                isFrictionZero = true;
            }
        }
        else if (IsGroundDetected())
        {
            if (isFrictionZero)
            {
                RestoreOriginalFriction();
                isFrictionZero = false;
            }
        }
    }

    public void ExitBlackHoleAbility()
    {
        stateMachine.ChangeState(airState);
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword() 
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    public IEnumerator BusyFor(float _second)
    {
        isBusy = true;
        yield return new WaitForSeconds(_second);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    #region "DashCheck"

    public void SetDash(float direction)
    {
        dashDir = direction;
    }

    private void CheckForDashInput()
    {
        //判断技能是否解锁
        if (!skill.dash.dashUnlocked)
            return;

        switch (stateMachine.currentState)
        {
            case PlayerBlackholeState:
                return;
            case PlayerAimSwordState:
                return;
            case PlayerCatchSwordState:
                return;
            case PlayerDeadState: 
                return;
        }

        if (Input.GetKeyDown(KeyCode.L) && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");
            
            if (dashDir == 0)
            {
                dashDir = facingDir;
            }
            stateMachine.ChangeState(dashState);
        }
    }
    #endregion  

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(dead);
    }
}
