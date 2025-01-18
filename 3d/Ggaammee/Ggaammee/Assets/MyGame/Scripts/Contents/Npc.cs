using UnityEngine;
using UnityEngine.Playables;

public class Npc : MonoBehaviour
{
    public NpcDialogue dialogue;
    [SerializeField] private PlayableDirector playableDirector;

    public void Interact()
    {
        if (dialogue == null) return;
        DialogueUI manager = FindObjectOfType<DialogueUI>();
        if (manager != null)
        {
            manager.StartDialogue(this);
        }
    }

    public void StartTimeline()
    {
        playableDirector.Play();
    }
}