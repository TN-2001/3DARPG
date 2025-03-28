using System;

/*
    StateMachine（ジェネリック）
*/

public class StateMachine<T> {
    public T Owner { get; }
    private StateBase<T> currentState = null;

    public Type PreviousStateType { get; private set; } = null;
    public Type CurrentStateType { get; private set; } = null;
    public Type NextStateType { get; private set; } = null;


    public StateMachine(T owner) {
        Owner = owner;
    }

    public void OnUpdate() {
        currentState.OnUpdate();
    }

    public void ChangeState(StateBase<T> nextState) {
        NextStateType = nextState.GetType();

        if (currentState != null) {
            currentState.OnEnd();
            PreviousStateType = currentState.GetType();
        }

        currentState = nextState;
        CurrentStateType = currentState.GetType();
        currentState.StateMachine = this;
        currentState.OnStart();
    }

    public void OnEnd() {
        currentState?.OnEnd();
    }
}

public abstract class StateBase<T> {
    public StateMachine<T> StateMachine;
    protected T Owner => StateMachine.Owner;

    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnEnd() { }
}
