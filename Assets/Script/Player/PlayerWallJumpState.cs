using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 此类功能不使用（滑墙跳跃功能已有实现）
 此类功能不使用（滑墙跳跃功能已有实现）
 此类功能不使用（滑墙跳跃功能已有实现）
 此类功能不使用（滑墙跳跃功能已有实现）
 此类功能不使用（滑墙跳跃功能已有实现）
 此类功能不使用（滑墙跳跃功能已有实现）
 此类功能不使用（滑墙跳跃功能已有实现）
 此类功能不使用（滑墙跳跃功能已有实现）
 */
public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = .4f;
        player.SetVelocity(5 * -player.facingDir, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
