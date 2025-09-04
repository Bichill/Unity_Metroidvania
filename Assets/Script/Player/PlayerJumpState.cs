using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 刷新空中连击次数
        player.comboInAirCount = 3;

        AudioManager.instance.PlaySFX(11, player.transform, Random.Range(1.1f, 1.3f), 0.15f);
        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.J) && player.comboInAirCount > 0)
        {
            stateMachine.ChangeState(player.primaryAttack);
        }

        if (Input.GetKeyDown(KeyCode.R) && SkillManager.instance.blackHole.CanUseSkill())
        {
            stateMachine.ChangeState(player.blackHole);
        }

        if (xInput != 0 && !player.isBusy)
        {
            player.SetVelocity(player.moveSpeed * xInput, rb.velocity.y);
        }
        if (rb.velocity.y < 0)
        {
            player.stateMachine.ChangeState(player.airState);
        }
        if ((Input.GetKeyDown(KeyCode.K) && player.jumpCount > 0 && !player.isBusy) 
            || (Input.GetKeyDown(KeyCode.K) && player.IsGroundDetected() && !player.isBusy))
        {
            player.jumpCount--;
            stateMachine.ChangeState(player.jumpState);
        }
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
