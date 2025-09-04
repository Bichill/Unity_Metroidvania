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
        {
            player.jumpCount = 2;
            player.comboInAirCount = 3;
        }
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
            float jumpForce = PlayerManager.instance.player.jumpForce;
            float jumpDirection = -player.facingDir; // å‘å¢™å¤–è·³è·ƒï¼Œæ‰€ä»¥æ–¹å‘ä¸é¢å‘ç›¸å
            Vector2 jumpVector = new Vector2(jumpDirection * jumpForce * 0.5f, jumpForce);
            player.SetVelocity(jumpVector.x, jumpVector.y);
            player.StartCoroutine(player.BusyFor(0.1f)); // Ìí¼Ó0.1ÃëµÄŸo·¨¿ØÖÆ•rég
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
