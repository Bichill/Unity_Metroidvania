using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(9, player.transform, 1.1f, 2f);
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(9);
    }
    
    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput*player.moveSpeed, rb.velocity.y);

        if (xInput==0) 
        { 
            stateMachine.ChangeState(player.idleState);
        }
    }
}
