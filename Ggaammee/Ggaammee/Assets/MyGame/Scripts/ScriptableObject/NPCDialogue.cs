using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialogue", menuName = "Dialogue/NPCDialogue")]
public class NPCDialogue : ScriptableObject
{
    public DialogueModule startModule; // 시작 모듈
    public string npcName; // NPC 이름 (UI 표시용)

    public DialogueModule GetStartModule()
    {
        return startModule;
    }
}