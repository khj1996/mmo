using System;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button[] choiceButtons;

    private Npc currentInteractNpc;
    private DialogueModule currentModule;
    private int currentLineIndex;
    private bool isDialogueActive;

    private void Start()
    {
        dialogueUI.SetActive(false);
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    public void StartDialogue(Npc npc)
    {
        currentInteractNpc = npc;
        npcNameText.text = npc.dialogue.npcName;
        LoadModule(npc.dialogue.GetStartModule());
        dialogueUI.SetActive(true);
        isDialogueActive = true;
        Managers.GameStateManager.SetState(GameState.InConversation);
    }

    private void LoadModule(DialogueModule module)
    {
        if (module == null)
        {
            EndDialogue();
            return;
        }

        module.enterAction?.Execute();


        currentModule = module;
        currentLineIndex = 0;
        ClearChoices();
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineIndex < currentModule.dialogueLines.Length)
        {
            dialogueText.text = currentModule.dialogueLines[currentLineIndex];
            HideChoices();
        }
        else
        {
            ShowChoices();
        }
    }

    private void NextLine()
    {
        if (currentModule.dialogueTimelineCues.FirstOrDefault(x => x.LineNumber == currentLineIndex) != default)
        {
            currentInteractNpc?.StartTimeline();
        }


        currentLineIndex++;

        if (currentLineIndex < currentModule.dialogueLines.Length)
        {
            ShowCurrentLine();
        }
        else if (currentModule.choices.Length == 0)
        {
            EndDialogue();
            Managers.GameStateManager.SetState(GameState.Normal);
        }
        else
        {
            ShowChoices();
        }
    }

    private void ShowChoices()
    {
        HideChoices();

        for (var index = 0; index < currentModule.choices.Length; index++)
        {
            var choice = currentModule.choices[index];
            if (index >= choiceButtons.Length)
            {
                break;
            }

            var button = choiceButtons[index];
            button.gameObject.SetActive(true);

            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.choiceText;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                choice.endAction?.Execute();

                DialogueModule nextModule = choice.GetNextModule();

                LoadModule(nextModule);

                if (nextModule == null && choice.endAction is not OpenShopAction)
                {
                    Managers.GameStateManager.SetState(GameState.Normal);
                }
            });
        }
    }

    private void HideChoices()
    {
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void ClearChoices()
    {
        foreach (var button in choiceButtons)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    }

    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
        currentInteractNpc = null;
        currentModule = null;
        isDialogueActive = false;
    }
}