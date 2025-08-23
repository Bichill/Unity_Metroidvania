using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private Enemy_Skeleton enemy;
    private int moveDir;
    private float flipCoolDown;

    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        flipCoolDown = .1f;
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //首先判断追击条件，防止追击跳崖的情况出现，因此优先级最高
        if (!enemy.IsGroundDetected())
        {
            stateMachine.ChangeState(enemy.moveState);
        }

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance && CanAttack())
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }
        else 
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position,enemy.transform.position) > 10)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }


        if (flipCoolDown <= 0f)
        {
            flipCoolDown = 0.5f;
        }
        flipCoolDown -= Time.deltaTime;

        if (player.position.x > enemy.transform.position.x && flipCoolDown <= 0f)
        {
            moveDir = 1;
        }
        else if (player.position.x < enemy.transform.position.x && flipCoolDown <= 0f)
        {
            moveDir = -1;
        }
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    private bool CanAttack()
    {
        if (Time.time - enemy.lastAttackTime > enemy.attackCooldown)
        {
            enemy.lastAttackTime = Time.time;
            return true;
        }
        else return false;
    }
}
