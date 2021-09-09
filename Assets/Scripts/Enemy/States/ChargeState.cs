using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeState : State
{
    protected D_ChargeState stateData;

    protected bool isPlayerInMinAgroRange;
    protected bool isDetectingLedge;
    protected bool isDetectingWall;
    protected bool isChargedTimeOver;
    protected bool performCloseRangeAction;

    public ChargeState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_ChargeState _stateData) : base(_entity, _stateMachine, _animBoolName)
    {
        this.stateData = _stateData;
    }


    public override void Enter()
    {
        base.Enter();

        isChargedTimeOver = false;
  
        entity.SetVelocity(stateData.chargeSpeed);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (Time.time >= startTime + stateData.chargeTime)
        {
            isChargedTimeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
        
        isDetectingWall = entity.CheckWall();
        isDetectingLedge = entity.CheckLedge();

        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
    }
}
