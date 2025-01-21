using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStateMachine<T> where T : CreatureController
{
    private State<T> _currentState; // 현재 상태
    private StateCache<T> _stateCache = new StateCache<T>(); // 상태 캐싱
    private Dictionary<(Type, Type), Func<bool>> _transitions = new Dictionary<(Type, Type), Func<bool>>(); // 상태 전이 조건

    public State<T> CurrentState => _currentState;

    //상태 변경
    public void ChangeState(Type stateType)
    {
        _currentState?.OnExit();
        _currentState = _stateCache.GetState(stateType);
        _currentState.OnEnter();
    }

    //상태 추가
    public void AddState(State<T> state)
    {
        _stateCache.AddState(state);
    }

    //상태 전이 조건
    public void AddTransition<U, V>(Func<bool> condition) where U : State<T> where V : State<T>
    {
        _transitions[(typeof(U), typeof(V))] = condition;
    }

    //글로벌 상태 전이 조건
    public void AddGlobalTransition<V>(Func<bool> condition) where V : State<T>
    {
        foreach (var stateType in _stateCache.GetAllStateTypes())
        {
            if (stateType != typeof(V))
            {
                _transitions[(stateType, typeof(V))] = condition;
            }
        }
    }

    //상태에 따른 업데이트 실행
    public void Update()
    {
        if (_currentState == null)
            return;

        foreach (var transition in _transitions)
        {
            if (transition.Key.Item1 == _currentState.GetType() && transition.Value())
            {
                ChangeState(transition.Key.Item2);
                break;
            }
        }

        _currentState?.OnUpdate();
    }
}

//상태 캐싱
public class StateCache<T> where T : CreatureController
{
    private Dictionary<Type, State<T>> _stateCache = new Dictionary<Type, State<T>>();

    public void AddState(State<T> state)
    {
        if (!_stateCache.ContainsKey(state.GetType()))
        {
            _stateCache.Add(state.GetType(), state);
        }
    }

    public State<T> GetState(Type stateType)
    {
        return _stateCache[stateType];
    }

    public IEnumerable<Type> GetAllStateTypes()
    {
        return _stateCache.Keys;
    }
}