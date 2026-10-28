using UnityEngine;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Interaction;
using UnityEngine.Events;

public class DateManager : MonoBehaviour
{
    [SerializeField] private MoneyRequestSystem.TargetCharacter currentCharacter;

    [Header("Interrogation")]
    [SerializeField] private int interrogationCooldownDays = 7;
    [SerializeField, Min(0)] private int interrogationPassScore = 2;
    [SerializeField, Range(0, 100)] private int suspicionAfterSuccessfulInterrogation = 70;

    [Header("Game Over")]
    [SerializeField] private UnityEvent onInterrogationFailed = new UnityEvent();


    [Header("Date Dialogues")]
    [SerializeField] private DialogueContainer vtuberDateDialogue;
    [SerializeField] private DialogueContainer olDateDialogue;
    [SerializeField] private DialogueContainer ceoDateDialogue;

    [Header("Interrogation Dialogues")]
    [SerializeField] private DialogueContainer vtuberInterrogationDialogue;
    [SerializeField] private DialogueContainer olInterrogationDialogue;
    [SerializeField] private DialogueContainer ceoInterrogationDialogue;

    
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
        StartSelectedDate(
            MoneyRequestSystem.TargetCharacter.Vtuber
        );
    }

    public void SetOL()
    {
        StartSelectedDate(
            MoneyRequestSystem.TargetCharacter.OL
        );
    }

    public void SetCEO()
    {
        StartSelectedDate(
            MoneyRequestSystem.TargetCharacter.CEO
        );
    }

    private bool IsInterrogationAvailable(
        MoneyRequestSystem.TargetCharacter character)
    {
        int currentDay =
            DialogueVariableData.Instance.Get<int>("Day");

        string lastInterrogationVariable = character switch
        {
            MoneyRequestSystem.TargetCharacter.Vtuber
                => "VtuberLastInterrogationDay",

            MoneyRequestSystem.TargetCharacter.OL
                => "OLLastInterrogationDay",

            MoneyRequestSystem.TargetCharacter.CEO
                => "CEOLastInterrogationDay",

            _ => ""
        };

        int lastInterrogationDay =
            DialogueVariableData.Instance.Get<int>(
                lastInterrogationVariable
            );

        int daysSinceLastInterrogation =
            currentDay - lastInterrogationDay;

        return daysSinceLastInterrogation >= interrogationCooldownDays;
    }

    private int GetCharacterSuspicion(
        MoneyRequestSystem.TargetCharacter character)
    {
        string suspicionVariable = character switch
        {
            MoneyRequestSystem.TargetCharacter.Vtuber
                => "VtuberSuspicion",

            MoneyRequestSystem.TargetCharacter.OL
                => "OLSuspicion",

            MoneyRequestSystem.TargetCharacter.CEO
                => "CEOSuspicion",

            _ => ""
        };

        return DialogueVariableData.Instance.Get<int>(
            suspicionVariable
        );
    }

    private DialogueContainer GetInterrogationDialogue(
        MoneyRequestSystem.TargetCharacter character)
    {
        return character switch
        {
            MoneyRequestSystem.TargetCharacter.Vtuber
                => vtuberInterrogationDialogue,

            MoneyRequestSystem.TargetCharacter.OL
                => olInterrogationDialogue,

            MoneyRequestSystem.TargetCharacter.CEO
                => ceoInterrogationDialogue,

            _ => null
        };
    }

    private DialogueContainer GetDateDialogue(
        MoneyRequestSystem.TargetCharacter character)
    {
        int suspicion = GetCharacterSuspicion(character);

        // Normal date
        if (suspicion < 100)
        {
            return character switch
            {
                MoneyRequestSystem.TargetCharacter.Vtuber
                    => vtuberDateDialogue,

                MoneyRequestSystem.TargetCharacter.OL
                    => olDateDialogue,

                MoneyRequestSystem.TargetCharacter.CEO
                    => ceoDateDialogue,

                _ => null
            };
        }

        // Suspicion is 100: check interrogation cooldown
        if (IsInterrogationAvailable(character))
        {
            return GetInterrogationDialogue(character);
        }

        // Suspicion is 100 and interrogation is unavailable
        Debug.LogWarning(
            $"GAME OVER: {character} caught the player!"
        );

        return null;
    }

    private void StartSelectedDate(
        MoneyRequestSystem.TargetCharacter character)
    {
        DialogueContainer selectedDialogue = GetDateDialogue(character);

        if (selectedDialogue == null)
        {
            return;
        }

         // Reset score when starting an interrogation
        if (GetCharacterSuspicion(character) >= 100)
        {
            DialogueVariableData.Instance.Set(
                "CurrentInterrogationScore",
                0
            );
        }

        SetCurrentCharacter(character);

        dateSelectionUI.SetActive(false);

        dialogueLauncher.PlayDialogue(selectedDialogue);
    }

    private void ChangeInterrogationScore(int amount)
    {
        int currentScore =
            DialogueVariableData.Instance.Get<int>(
                "CurrentInterrogationScore"
            );

        int newScore = currentScore + amount;

        DialogueVariableData.Instance.Set(
            "CurrentInterrogationScore",
            newScore
        );

        Debug.Log(
            $"Interrogation Score: {currentScore} -> {newScore}"
        );
    }

    public void AddVeryConvincingInterrogationPoints()
    {
        ChangeInterrogationScore(2);
    }

    public void AddGoodInterrogationPoints()
    {
        ChangeInterrogationScore(1);
    }

    public void AddSuspiciousInterrogationPoints()
    {
        ChangeInterrogationScore(-1);
    }

    public void AddExtremelySuspiciousInterrogationPoints()
    {
        ChangeInterrogationScore(-2);
    }

    public void ResolveInterrogation()
    {
        int score = DialogueVariableData.Instance
            .Get<int>("CurrentInterrogationScore");

        bool passed = score >= interrogationPassScore;

        DialogueVariableData.Instance.Set(
            "InterrogationPassed",
            passed
        );

        Debug.Log(
            $"Interrogation finished! Score: {score}, Passed: {passed}"
        );

        // If the player failed, trigger the placeholder
        // Game Over message for now.
        if (!passed)
        {
            Debug.LogError(
                $"GAME OVER: {CurrentCharacter} caught the player!"
            );

            onInterrogationFailed.Invoke();

            return;
        }

        // Find the variables belonging to the current character.
        string suspicionVariable;
        string lastInterrogationDayVariable;

        switch (CurrentCharacter)
        {
            case MoneyRequestSystem.TargetCharacter.Vtuber:
                suspicionVariable = "VtuberSuspicion";
                lastInterrogationDayVariable =
                    "VtuberLastInterrogationDay";
                break;

            case MoneyRequestSystem.TargetCharacter.OL:
                suspicionVariable = "OLSuspicion";
                lastInterrogationDayVariable =
                    "OLLastInterrogationDay";
                break;

            case MoneyRequestSystem.TargetCharacter.CEO:
                suspicionVariable = "CEOSuspicion";
                lastInterrogationDayVariable =
                    "CEOLastInterrogationDay";
                break;

            default:
                Debug.LogError(
                    $"Unknown interrogation target: {CurrentCharacter}"
                );
                return;
        }

        // Successful interrogation: reduce suspicion to 70.
        DialogueVariableData.Instance.Set(
            suspicionVariable,
            suspicionAfterSuccessfulInterrogation
        );

        // Record the day this interrogation was passed.
        int currentDay = DialogueVariableData.Instance
            .Get<int>("Day");

        DialogueVariableData.Instance.Set(
            lastInterrogationDayVariable,
            currentDay
        );

        Debug.Log(
            $"{CurrentCharacter} interrogation passed! " +
            $"Suspicion reduced to {suspicionAfterSuccessfulInterrogation}. " +
            $"Next interrogation available after the cooldown."
        );
    }
}