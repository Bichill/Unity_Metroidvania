using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }

    private float lastTimeAttacked;
    private float comboWindow = 1;
    private float originalGravityScale; // ����

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // ��¼��ǰ����״̬
        player.SetLastAttackState(this);

        originalGravityScale = rb.gravityScale; // ��¼ԭʼ����
        
        //���ڿ���ʱ
        if (!player.IsGroundDetected())
        {
            rb.gravityScale = 2.5f;
            player.comboInAirCount--;//�����������ƽa
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
        rb.gravityScale = originalGravityScale; // �ָ�ԭʼ����
        player.StartCoroutine("BusyFor", .05f); // ������ҡ
        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        // ��ӹ���ȡ�����ڣ��ڹ�������������Ծ
        // ���ǲ������ڵ���ȡ������
        if (stateTimer < 0.025f && Input.GetKeyDown(KeyCode.K) && player.jumpCount > 0 && !player.IsGroundDetected())
        {
            player.jumpCount--;
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        // ͬʱ���ٿ���ƽa��ҡ
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
