using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<TOwner>
{
    public TOwner Owner { get; }
    public StateBase<TOwner> currentState { get; private set; }

    public StateMachine(TOwner owner)
    {
        Owner = owner;
    }

    public void OnUpdate()
    {
        currentState.OnUpdate();
    }

    public void ChangeState(StateBase<TOwner> nextState)
    {
        currentState?.OnEnd();
        currentState = nextState;
        currentState.StateMachine = this;
        currentState.OnStart();
    }
}

public abstract class StateBase<TOwner>
{
    public StateMachine<TOwner> StateMachine;
    protected TOwner Owner => StateMachine.Owner;

    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnEnd() { }
}
