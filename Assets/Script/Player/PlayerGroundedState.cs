using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        if(player.IsGroundDetected())//��ɫֻ���ڵ��ϲ�ˢ����Ծ����
            player.jumpCount = 1;
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.R) && SkillManager.instance.blackHole.CanUseSkill())
        {
            stateMachine.ChangeState(player.blackHole);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() &&SkillManager.instance.sword.swordUnlocked)
        {
            stateMachine.ChangeState(player.aimSword);
        }

        if (Input.GetKeyDown(KeyCode.H) && SkillManager.instance.parry.parryUnlocked)
        {
            stateMachine.ChangeState(player.counterAttack);
        }

        if (Input.GetKey(KeyCode.J))
        {
            stateMachine.ChangeState(player.primaryAttack);
        }

        if (Input.GetKeyDown(KeyCode.K) && player.IsGroundDetected()) 
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (Input.GetKeyDown(KeyCode.K) && player.jumpCount>0)
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (!player.IsGroundDetected()) 
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }
        //����õȽ������������ٶ�Ϊ0ʱ�����߳������Ͷ������ʱ�������ջ�
        if (player.sword.GetComponent<Rigidbody2D>().velocity == new Vector2(0, 0))
        {
            player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        }
        return false;
    }
}
