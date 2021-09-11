using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_LookForPlayer : LookForPlayerState
{
    public Enemy1 enemy1;
        
    public E1_LookForPlayer(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_LookForPlayer _stateData, Enemy1 _enemy1) : base(_entity, _stateMachine, _animBoolName, _stateData)
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

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy1.playerDetectedState);
        }
        else if(isAllTurnsTimeDone)
        {
            stateMachine.ChangeState(enemy1.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }
}
