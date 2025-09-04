using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
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
        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * xInput * 0.8f, rb.velocity.y);
        }
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (Input.GetKeyDown(KeyCode.K) && player.jumpCount > 0)
        {
            player.jumpCount--;
            stateMachine.ChangeState(player.jumpState);
        }
    }
}
