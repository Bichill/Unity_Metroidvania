using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundedState : EnemyState
{
    protected Enemy_Skeleton enemy;

    protected Transform player;

    private float lastStateChangeTime;

    public SkeletonGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //�����⵽ǽ����
        if (enemy.IsPlayerDetected())
        {
            // �������һ���ӳ٣�����Ƶ���л�
            if (Time.time - lastStateChangeTime > 1.2f)
            {
                stateMachine.ChangeState(enemy.battleState);
                lastStateChangeTime = Time.time;
            }
        }
    }
}
