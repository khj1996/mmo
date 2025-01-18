using UnityEngine;

public class DialogueAction : ScriptableObject
{
    public virtual void Execute()
    {
    }
}


public class ExecuteFunctionAction : DialogueAction
{
    public override void Execute()
    {
    }
}