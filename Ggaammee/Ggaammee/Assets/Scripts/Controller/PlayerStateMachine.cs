using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    private State _currentState;
    private StateCache _stateCache = new StateCache();
    private Dictionary<(Type, Type), Func<bool>> _transitions = new Dictionary<(Type, Type), Func<bool>>();

    public State CurrentState => _currentState;

    // 현재 상태를 변경하는 메서드
    public void ChangeState(Type stateType)
    {
        _currentState?.OnExit();
        _currentState = _stateCache.GetState(stateType);
        _currentState.OnEnter();
    }

    // 상태를 추가하는 메서드
    public void AddState(State state)
    {
        _stateCache.AddState(state);
    }

    // 상태 전이 조건을 추가하는 메서드
    public void AddTransition<T, U>(Func<bool> condition) where T : State where U : State
    {
        _transitions[(typeof(T), typeof(U))] = condition;
    }

    // 상태 업데이트 및 전이 체크
    public void Update()
    {
        foreach (var transition in _transitions)
        {
            if (transition.Key.Item1 == _currentState.GetType() && transition.Value())
            {
                ChangeState(transition.Key.Item2);
                break;
            }
        }

        Debug.Log(_currentState);
        _currentState?.OnUpdate();
    }
}

public class StateCache
{
    private Dictionary<Type, State> _stateCache = new Dictionary<Type, State>();

    public void AddState(State state)
    {
        if (!_stateCache.TryGetValue(state.GetType(), out _))
        {
            _stateCache.Add(state.GetType(), state);
        }
    }

    public State GetState(Type stateType)
    {
        return _stateCache[stateType];
    }
}

public abstract class State
{
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
}

public class IdleState : State
{
    private PlayerControllerFSM _controller;

    public IdleState(PlayerControllerFSM controller)
    {
        _controller = controller;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        _controller.JumpAndGravity();
        _controller.GroundedCheck();
        _controller.Move();
    }
}

public class MoveState : State
{
    private PlayerControllerFSM _controller;

    public MoveState(PlayerControllerFSM controller)
    {
        _controller = controller;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        _controller.JumpAndGravity();
        _controller.GroundedCheck();
        _controller.Move();
    }
}

public class AttackState : State
{
    private PlayerControllerFSM _controller;

    public AttackState(PlayerControllerFSM controller)
    {
        _controller = controller;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        // Attack 상태에서 해야 할 로직
    }
}

public class CrouchState : State
{
    private PlayerControllerFSM _controller;

    public CrouchState(PlayerControllerFSM controller)
    {
        _controller = controller;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        _controller.Move();
    }
}

public class JumpState : State
{
    private PlayerControllerFSM _controller;

    public JumpState(PlayerControllerFSM controller)
    {
        _controller = controller;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        _controller.JumpAndGravity();
        _controller.GroundedCheck();
        _controller.Move();
    }
}

public class GetHitState : State
{
    private PlayerControllerFSM _controller;

    public GetHitState(PlayerControllerFSM controller)
    {
        _controller = controller;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        // Attack 상태에서 해야 할 로직
    }
}