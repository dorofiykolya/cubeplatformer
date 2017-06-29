using System.Collections.Generic;
using UnityEngine;

public class StateM_State
{
    private StateM_Transition _currentTransition;
    private readonly NSM_StateMachine _ownerMachine;

    public NSM_TransitionDelegate DoBeforeEntering = delegate { };
    public NSM_TransitionDelegate DoBeforeLeaving = delegate { };
    public NSM_TransitionDelegate StateLoop = delegate { };

    private readonly List<StateM_Transition> _transitions = new List<StateM_Transition>();

    public StateM_State(NSM_StateMachine ownerMachine, string name)
    {
        _ownerMachine = ownerMachine;
    }

    public StateM_Transition CurrentTransition
    {
        set { _currentTransition = value; }
    }

    public void LeaveState(StateM_State newState)
    {
        DoBeforeLeaving.Invoke();
        _ownerMachine.CurrentState = newState;
        DoBeforeEntering.Invoke();
    }

    public StateM_Transition AddNewTransition(StateM_Transition newTransition)
    {
        _transitions.Add(newTransition);
        return newTransition;
    }

    public void UpdateState()
    {
        if (_currentTransition != null)
        {
            _currentTransition.UpdateTransition(this);
        }
        else
        {
            for (var i = 0; i < _transitions.Count; ++i)
            {
                if (_transitions[i].UpdateTransition(this))
                    return;
            }

            StateLoop.Invoke();
        }
    }
}

public class StateM_Transition
{
    private readonly StateM_State _targetState;

    private bool _isTransiting;
    private float _transitionDelta;

    private readonly bool _hasTransitionTime;
    public NSM_TransitionDelegate OnEndTransit = delegate { };
    public NSM_TransitionDelegate OnStartTransit = delegate { };

    public NSM_TransitionTestDelegate TestAllowTransit = delegate { return false; };
    private readonly float _transitionDuration = 1;
    public NSM_TransitionDelegatePos TransitPosition = delegate { };

    public StateM_Transition(StateM_State targetState)
    {
        _targetState = targetState;
    }

    public StateM_Transition(StateM_State targetState, bool hasTransitionTime, float transitionDuration)
    {
        _targetState = targetState;
        _hasTransitionTime = hasTransitionTime;
        _transitionDuration = transitionDuration;
    }

    private bool AllowTransit
    {
        get { return TestAllowTransit.Invoke(); }
    }

    public bool UpdateTransition(StateM_State ownerState)
    {
        if (_isTransiting)
        {
            _transitionDelta += Time.deltaTime/_transitionDuration;
            _transitionDelta = Mathf.Clamp(_transitionDelta, 0, 1);
            TransitPosition.Invoke(_transitionDelta);

            if (_transitionDelta == 1)
            {
                _isTransiting = false;
                FinishTransit(ownerState);
            }
            return true;
        }
        else if (AllowTransit)
        {
            ownerState.CurrentTransition = this;
            OnStartTransit.Invoke();

            if (_hasTransitionTime)
            {
                _isTransiting = true;
                _transitionDelta = 0;
            }
            else
            {
                FinishTransit(ownerState);
            }
            return true;
        }
        return false;
    }

    private void FinishTransit(StateM_State ownerState)
    {
        ownerState.CurrentTransition = null;
        OnEndTransit.Invoke();
        ownerState.LeaveState(_targetState);
    }
}

public delegate void NSM_TransitionDelegate();
public delegate void NSM_TransitionDelegatePos(float delta);
public delegate bool NSM_TransitionTestDelegate();

public class NSM_StateMachine
{
    private StateM_State _currentState;

    public StateM_State CurrentState
    {
        set { _currentState = value; }
    }

    public StateM_State AddNewState(string stateName)
    {
        return new StateM_State(this, stateName);
    }

    public void InitStateMachine(StateM_State startState)
    {
        _currentState = startState;
    }

    public void UpdateStateMachine()
    {
        _currentState.UpdateState();
    }
}