using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }

    private float lastTimeAttacked;
    private float comboWindow = 1;
    private float originalGravityScale; // 新增

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 记录当前攻击状态
        player.SetLastAttackState(this);

        originalGravityScale = rb.gravityScale; // 记录原始重力
        
        //当在空中时
        if (!player.IsGroundDetected())
        {
            rb.gravityScale = 2.5f;
            player.comboInAirCount--;//避免空中无限平a
        }

        if (comboCounter > 2 || Time.time >= comboWindow + lastTimeAttacked)
            comboCounter = 0;

        float attackDir = xInput != 0 ? xInput : player.facingDir; 
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);
        player.anim.SetInteger("ComboCounter", comboCounter);
        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravityScale; // 恢复原始重力
        player.StartCoroutine("BusyFor", .05f); // 攻击后摇
        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        // 添加攻击取消窗口，在攻击后半段允许跳跃
        // 但是不允许在地面取消后腰
        if (stateTimer < 0.025f && Input.GetKeyDown(KeyCode.K) && player.jumpCount > 0 && !player.IsGroundDetected())
        {
            player.jumpCount--;
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        // 同时减少空中平a后摇
        if (stateTimer < 0.05f && Input.GetKeyDown(KeyCode.J) && player.comboInAirCount > 0 && !player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.primaryAttack);
            return;
        }


        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (stateTimer<0)
        {
            player.SetZeroVelocity();
        }
    }
}
