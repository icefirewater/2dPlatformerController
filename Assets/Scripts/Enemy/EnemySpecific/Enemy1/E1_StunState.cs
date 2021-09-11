using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_StunState : StunState
{
    private Enemy1 enemy1;

    public E1_StunState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_StunState _stateData, Enemy1 _enemy1) : base(_entity, _stateMachine, _animBoolName, _stateData)
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

        if (isStunTimeOver)
        {
            if (performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy1.meleeAttackState);
            }
            else if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy1.chargeState);
            }
            else
            {
                enemy1.lookForPlayerState.SetTurnImmediately(true);
                stateMachine.ChangeState(enemy1.lookForPlayerState);
            }
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
