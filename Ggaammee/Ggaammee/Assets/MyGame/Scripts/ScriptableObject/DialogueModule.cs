using UnityEngine;

[CreateAssetMenu(fileName = "DialogueModule", menuName = "Dialogue/DialogueModule")]
public class DialogueModule : ScriptableObject
{
    [TextArea]
    public string[] dialogueLines; // 대화 내용
    public DialogueChoice[] choices; // 선택지 배열
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; // 선택지 텍스트
    public DialogueModule nextModule; // 선택 후 이동할 다음 모듈
    public DialogueAction action; // 선택 후 실행할 액션 (함수 실행, 상점 열기 등)
}