using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAction", menuName = "Dialogue/DialogueAction")]
public class DialogueAction : ScriptableObject
{
    public virtual void Execute()
    {
    }
}

public class OpenShopAction : DialogueAction
{
    public override void Execute()
    {
        Debug.Log("상점이 열립니다!");
    }
}

public class ExecuteFunctionAction : DialogueAction
{
    public override void Execute()
    {
        Debug.Log("함수가 실행됩니다!");
    }
}