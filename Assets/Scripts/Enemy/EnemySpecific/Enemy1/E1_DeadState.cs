using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_DeadState : DeadState
{
    private Enemy1 enemy1;

    public E1_DeadState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_DeadState _stateData, Enemy1 _enemy1) : base(_entity, _stateMachine, _animBoolName, _stateData)
    {
        this.enemy1 = _enemy1;
    }

    public override void DoChecks()
    {
        base.DoChecks();
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
