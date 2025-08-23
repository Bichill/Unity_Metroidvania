using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallslidState : PlayerState
{
    public PlayerWallslidState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (player.IsWallDetected())
            player.jumpCount = 2;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (xInput != 0 && player.facingDir != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (yInput < 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y );
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y * .7f);
        }

        if (player.IsGroundDetected() || !player.IsWallDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (Input.GetKeyDown(KeyCode.K) && player.jumpCount>0)
        {
            player.jumpCount--;
            // 计算45度角跳跃的向量
            float jumpForce = PlayerManager.instance.player.jumpForce;
            float jumpDirection = -player.facingDir; // 向墙外跳跃，所以方向与面向相反
            Vector2 jumpVector = new Vector2(jumpDirection * jumpForce * 0.5f, jumpForce);
            player.SetVelocity(jumpVector.x, jumpVector.y);
            player.StartCoroutine(player.BusyFor(0.1f)); // 添加0.2秒的无法控制时间
            stateMachine.ChangeState(player.jumpState);
        }

        if (Input.GetKeyDown(KeyCode.L) && !player.isBusy && SkillManager.instance.dash.dashUnlocked)
        {
            if (SkillManager.instance.dash.CanUseSkill())
            {
                player.SetDash(-player.facingDir);
                stateMachine.ChangeState(player.dashState);
            }
        }
    }
}
