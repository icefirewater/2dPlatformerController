using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected Transform attackPosition;

    protected bool isAnimationFinished;
    protected bool isPlayerInMinAgroRange;

    public AttackState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, Transform _attackPosition) : base(_entity, _stateMachine, _animBoolName)
    {
        this.attackPosition = _attackPosition;
    }

    public override void Enter()
    {
        base.Enter();

        isAnimationFinished = false;
        entity.atsm.attackState = this;
        entity.SetVelocity(0.0f);
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
   
    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public virtual void TriggerAttack()
    {

    }

    public virtual void FinishAttack()
    {
        isAnimationFinished = true;
    }
}
