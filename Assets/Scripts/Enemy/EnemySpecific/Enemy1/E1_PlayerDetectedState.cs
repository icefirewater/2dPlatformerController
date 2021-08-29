using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_PlayerDetectedState : PlayerDetectedState
{
    private Enemy1 enemy1;

    public E1_PlayerDetectedState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_PlayerDetectedState _stateData, Enemy1 _enemy1) : base(_entity, _stateMachine, _animBoolName, _stateData)
    {
        this.enemy1 = _enemy1;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isPlayerInMaxAgroRange)
        {
            enemy1.idleState.SetFlipAfterIdle(false);
            stateMachine.ChangeState(enemy1.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
