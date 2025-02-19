using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<TOwner>
{
    public TOwner Owner { get; }
    public StateBase<TOwner> CurrentState { get; private set; }

    public StateMachine(TOwner owner)
    {
        Owner = owner;
    }

    public void OnUpdate()
    {
        CurrentState.OnUpdate();
    }

    public void ChangeState(StateBase<TOwner> nextState)
    {
        CurrentState?.OnEnd();
        CurrentState = nextState;
        CurrentState.StateMachine = this;
        CurrentState.OnStart();
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
