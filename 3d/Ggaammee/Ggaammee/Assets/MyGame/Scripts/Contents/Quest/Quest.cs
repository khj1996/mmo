using System;

public abstract class Quest
{
    public QuestData Data { get; private set; }
    public event Action OnUpdateProgress;

    protected Quest(QuestData data)
    {
        Data = data;
    }

    public abstract void Subscribe();
    public abstract void Unsubscribe();
    public abstract bool CanComplete();
    public abstract string GetProgress();


    protected virtual void InvokeOnUpdateProgress()
    {
        OnUpdateProgress?.Invoke();
    }
}
