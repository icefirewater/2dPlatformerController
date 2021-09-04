using UnityEngine;

public class IdleState : State
{
    protected D_IdleState stateData;

    protected bool flipAfterIdle;
    protected bool isIdleTimeOver;
    protected bool isPlayerDetectedInMinAgroRange;
        
    protected float idleTime;
    public IdleState(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName, D_IdleState _stateData) : base(_entity, _stateMachine, _animBoolName)
    {
        this.stateData = _stateData;
    }


    public override void Enter()
    {
        base.Enter();
        entity.SetVelocity(0f);
        isIdleTimeOver = false;
        SetRandomIdleTime();
    }

    public override void Exit()
    {
        base.Exit();

        if (flipAfterIdle)
        {
            entity.Flip();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + idleTime)
        {
            isIdleTimeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void SetFlipAfterIdle(bool _flip)            // Setter for flipAfterIdle
    {
        flipAfterIdle = _flip;
    }

    private void SetRandomIdleTime()
    {
        idleTime = Random.Range(stateData.minIdleTime, stateData.maxIdleTime);
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerDetectedInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }
}
