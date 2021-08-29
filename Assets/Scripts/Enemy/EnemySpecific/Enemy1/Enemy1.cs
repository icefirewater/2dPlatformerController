using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Entity
{
    public E1_IdleState idleState { get; private set; }
    public E1_MoveState moveState { get; private set; }
    public E1_PlayerDetectedState playerDetectedState { get; private set; }

    [SerializeField] private D_IdleState idleStateData;                                                                   // get ref
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedData; 

    public override void Start()
    {
        base.Start();

        moveState = new E1_MoveState(this, stateMachine, "Move", moveStateData, this);                                          // create Instance
        idleState = new E1_IdleState(this, stateMachine, "Idle", idleStateData, this);
        playerDetectedState = new E1_PlayerDetectedState(this, stateMachine, "PlayerDetected", playerDetectedData, this);

        stateMachine.Initialize(moveState);
    }
}
