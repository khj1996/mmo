public abstract class Quest
{
    public QuestData Data { get; private set; }

    protected Quest(QuestData data)
    {
        Data = data;
    }

    public abstract void Subscribe();
    public abstract void Unsubscribe();
    public abstract bool IsComplete();
}
