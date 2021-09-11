using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_ChargeState : ChargeState
{

    private Enemy1 enemy1;

    public E1_ChargeState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_ChargeState _chargedState, Enemy1 _enemy1) : base(_entity, _stateMachine, _animBoolName, _chargedState)
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
            
        if (performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy1.meleeAttackState);
            }

        else if (isDetectingWall || !isDetectingLedge)
        {
            stateMachine.ChangeState(enemy1.lookForPlayerState);
        }   

        else if (isChargedTimeOver)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy1.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy1.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
