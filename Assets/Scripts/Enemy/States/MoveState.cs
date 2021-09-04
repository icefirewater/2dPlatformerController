using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected D_MoveState stateData;

    protected bool isTouchingWall;
    protected bool isDetectingLedge;
    protected bool isPlayerDetectedInMinAgroRange;
    public MoveState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_MoveState _stateData) : base(_entity, _stateMachine, _animBoolName)
    {
        this.stateData = _stateData;
    }


    public override void Enter()
    {
        base.Enter();

        entity.SetVelocity(stateData.movementSpeed);
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
        isDetectingLedge = entity.CheckLedge();
        isTouchingWall = entity.CheckWall();
        isPlayerDetectedInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }
}
