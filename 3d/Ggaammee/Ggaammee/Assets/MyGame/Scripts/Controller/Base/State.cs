public abstract class State<T>
{
    protected T _owner;

    public State(T owner)
    {
        _owner = owner;
    }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
}