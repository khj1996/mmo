using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAction", menuName = "Dialogue/DialogueAction")]
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
        Debug.Log("함수가 실행됩니다!");
    }
}