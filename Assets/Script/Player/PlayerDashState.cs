using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float originalGravityScale; // 新增

    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        originalGravityScale = rb.gravityScale; // 记录原始重力
        rb.gravityScale = 0f;
        player.skill.dash.CloneOnDash();
        stateTimer = player.dashDuration;
        player.isInvincible = true;
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, rb.velocity.y);
        rb.gravityScale = originalGravityScale; // 恢复原始重力
        player.isInvincible = false;
        player.skill.dash.CloneOnArrival();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
