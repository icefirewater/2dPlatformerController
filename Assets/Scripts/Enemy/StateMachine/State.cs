using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* State class has the every function that all the States should have so Enter, Exit, Normal Update function and Physics Update state.
 * So when we create a state like Idle or Move then those are going to be inherited from State class. In our Enemy class we are going to create object of the State */
public class State 
{
    protected FiniteStateMachine stateMachine;        // keep track of FiniteStateMachine that it belongs to
    protected Entity entity;                          // keep track of which entity it belongs to

    protected float startTime;

    protected string animBoolName;

    public State(Entity _entity, FiniteStateMachine _stateMachine, string _animBoolName)       // since we need to create Objects out of these States we need to create Constructor
    {
        this.entity = _entity;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    } 

    public virtual void Enter()                       // virtual means thats this function can be redefine in the Derived class
    {
        startTime = Time.time;
        entity.anim.SetBool(animBoolName, true);

        DoChecks();
    }

    public virtual void Exit()
    {
        entity.anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
}
