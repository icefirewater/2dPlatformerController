using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FiniteStateMachine class is going to keep track of what State the Enemy is currently in & run the correct code from that State. Keep track of current State.   
public class FiniteStateMachine
{
    public State currentState { get; private set; }         // Keep track of current state which has public getter and private setter
    
    public void Initialize(State _startingState)               
    {
        currentState = _startingState;
        currentState.Enter();
    }

    public void ChangeState(State _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
