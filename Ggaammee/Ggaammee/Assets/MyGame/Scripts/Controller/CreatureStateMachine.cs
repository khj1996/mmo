using System;
using System.Collections.Generic;

public class CreatureStateMachine<T> where T : CreatureController
{
    private CreatureData.State<T> _currentState;
    private StateCache<T> _stateCache = new StateCache<T>();
    private Dictionary<(Type, Type), Func<bool>> _transitions = new Dictionary<(Type, Type), Func<bool>>();

    public CreatureData.State<T> CurrentState => _currentState;

    public void ChangeState(Type stateType)
    {
        _currentState?.OnExit();
        _currentState = _stateCache.GetState(stateType);
        _currentState.OnEnter();
    }

    public void AddState(CreatureData.State<T> state)
    {
        _stateCache.AddState(state);
    }

    public void AddTransition<U, V>(Func<bool> condition) where U : CreatureData.State<T> where V : CreatureData.State<T>
    {
        _transitions[(typeof(U), typeof(V))] = condition;
    }

    public void AddGlobalTransition<V>(Func<bool> condition) where V : CreatureData.State<T>
    {
        foreach (var stateType in _stateCache.GetAllStateTypes())
        {
            if (stateType != typeof(V)) // 자기 자신으로 전환은 추가하지 않음
            {
                _transitions[(stateType, typeof(V))] = condition;
            }
        }
    }

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

        _currentState?.OnUpdate();
    }
}

public class StateCache<T> where T : CreatureController
{
    private Dictionary<Type, CreatureData.State<T>> _stateCache = new Dictionary<Type, CreatureData.State<T>>();

    public void AddState(CreatureData.State<T> state)
    {
        if (!_stateCache.ContainsKey(state.GetType()))
        {
            _stateCache.Add(state.GetType(), state);
        }
    }

    public CreatureData.State<T> GetState(Type stateType)
    {
        return _stateCache[stateType];
    }

    public IEnumerable<Type> GetAllStateTypes()
    {
        return _stateCache.Keys;
    }
}