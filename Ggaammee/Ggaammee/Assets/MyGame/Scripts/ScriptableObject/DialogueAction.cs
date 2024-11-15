using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAction", menuName = "Dialogue/DialogueAction")]
public class DialogueAction : ScriptableObject
{
    public virtual void Execute()
    {
        // 모듈화된 액션 실행 로직 (상점 열기, 함수 호출 등)
    }
}

public class OpenShopAction : DialogueAction
{
    public override void Execute()
    {
        // 상점 열기 로직
        Debug.Log("상점이 열립니다!");
    }
}

public class ExecuteFunctionAction : DialogueAction
{
    public override void Execute()
    {
        // 특정 함수 실행 로직
        Debug.Log("함수가 실행됩니다!");
    }
}