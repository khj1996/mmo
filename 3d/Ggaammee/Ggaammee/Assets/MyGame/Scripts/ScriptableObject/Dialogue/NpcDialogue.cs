using UnityEngine;

[CreateAssetMenu(fileName = "NpcDialogue", menuName = "ScriptableObjects/Dialogue/NpcDialogue")]
public class NpcDialogue : ScriptableObject
{
    public DialogueModule startModule; // 시작 모듈
    public string npcName; // NPC 이름 (UI 표시용)

    public DialogueModule GetStartModule()
    {
        return startModule;
    }
}