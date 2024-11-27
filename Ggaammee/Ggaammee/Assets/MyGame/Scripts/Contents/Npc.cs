using UnityEngine;

public class Npc : MonoBehaviour
{
    public NpcDialogue dialogue; 

    public void Interact()
    {
        if (dialogue != null)
        {
            DialogueUI manager = FindObjectOfType<DialogueUI>();
            if (manager != null)
            {
                manager.StartDialogue(dialogue);
            }
        }
        else
        {
            Debug.Log("대화 데이터가 없습니다!");
        }
    }
}