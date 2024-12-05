using UnityEngine;
using UnityEngine.Playables;

public class Npc : MonoBehaviour
{
    public NpcDialogue dialogue;
    [SerializeField] private PlayableDirector playableDirector;

    public void Interact()
    {
        if (dialogue != null)
        {
            DialogueUI manager = FindObjectOfType<DialogueUI>();
            if (manager != null)
            {
                manager.StartDialogue(this);
            }
        }
        else
        {
            Debug.Log("대화 데이터가 없습니다!");
        }
    }

    public void StartTimeline()
    {
        playableDirector.Play();
    }
}