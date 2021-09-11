using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : State
{
    protected D_PlayerDetectedState stateData;

    protected bool isPlayerInMinAgroRange;
    protected bool isPlayerInMaxAgroRange;
    protected bool performLongRangeAction;
    protected bool performCloseRangeAction;
    protected bool isDetectingLedge;

    public PlayerDetectedState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_PlayerDetectedState _stateData) : base(_entity, _stateMachine, _animBoolName)
    {
        this.stateData = _stateData;
    }


    public override void Enter()
    {
        base.Enter();

        performLongRangeAction = false;

        entity.SetVelocity(0.0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(Time.time >= startTime + stateData.LongRangeActionTime)
        {
            performLongRangeAction = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
     
    }
    public override void DoChecks()
    {
        base.DoChecks();

        isDetectingLedge = entity.CheckLedge();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();               // check if Player is in minimum agro range of Enemy
        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAgroRange();               // check if Player is in maximum agro range of Enemy

        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();          //check for melee attack
    }
}
