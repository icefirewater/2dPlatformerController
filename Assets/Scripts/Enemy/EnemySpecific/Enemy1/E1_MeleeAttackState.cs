using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_MeleeAttackState : MeleeAttackState
{

    private Enemy1 enemy1;

    public E1_MeleeAttackState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, Transform _attackPosition, D_MeleeAttack _stateData, Enemy1 _enemy1) : base(_entity, _stateMachine, _animBoolName, _attackPosition, _stateData)
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

    public override void FinishAttack()
    {
        base.FinishAttack();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
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

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }
}
