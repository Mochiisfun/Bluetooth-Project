using UnityEngine;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Interaction;

public class DateManager : MonoBehaviour
{
    [SerializeField] private MoneyRequestSystem.TargetCharacter currentCharacter;
    [SerializeField] private int interrogationCooldownDays = 7;

    [Header("Date Dialogues")]
    [SerializeField] private DialogueContainer vtuberDateDialogue;
    [SerializeField] private DialogueContainer olDateDialogue;
    [SerializeField] private DialogueContainer ceoDateDialogue;

    
    [Header("Dialogue Launcher")]
    [SerializeField] private DialogueLauncher dialogueLauncher;

    [Header("Date Selection UI")]
    [SerializeField] private GameObject dateSelectionUI;




    public MoneyRequestSystem.TargetCharacter CurrentCharacter
        => currentCharacter;

    public void SetCurrentCharacter(
        MoneyRequestSystem.TargetCharacter character)
    {
        currentCharacter = character;

        Debug.Log(
            $"Current date character: {currentCharacter}"
        );
    }

    public void SetVtuber()
    {
        SetCurrentCharacter(
            MoneyRequestSystem.TargetCharacter.Vtuber
        );

        dateSelectionUI.SetActive(false);

        dialogueLauncher.PlayDialogue(vtuberDateDialogue);
    }

    public void SetOL()
    {
        SetCurrentCharacter(
            MoneyRequestSystem.TargetCharacter.OL
        );

        dateSelectionUI.SetActive(false);

        dialogueLauncher.PlayDialogue(olDateDialogue);
    }

    public void SetCEO()
    {
        SetCurrentCharacter(
            MoneyRequestSystem.TargetCharacter.CEO
        );

        dateSelectionUI.SetActive(false);

        dialogueLauncher.PlayDialogue(ceoDateDialogue);
    }
}