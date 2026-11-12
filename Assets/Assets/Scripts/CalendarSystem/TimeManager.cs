using DialogueSystem.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public void AdvanceTimeSlot()
    {
        int currentTimeSlot =
            DialogueVariableData.Instance.Get<int>("TimeSlot");

        int currentDay =
            DialogueVariableData.Instance.Get<int>("Day");

        if (currentTimeSlot < 2)
        {
            // Move to the next time slot.
            DialogueVariableData.Instance.Set(
                "TimeSlot",
                currentTimeSlot + 1
            );
        }
        else
        {
            // Night has passed. Start a new day.
            DialogueVariableData.Instance.Set("TimeSlot", 0);

            DialogueVariableData.Instance.Set(
                "Day",
                currentDay + 1
            );

            // Check whether a debt collection visit is due.
            DebtManager debtManager = GetComponent<DebtManager>();

            if (debtManager != null)
            {
                debtManager.CheckForCollectionVisit();
            }
            else
            {
                Debug.LogError("DebtManager was not found!");
            }
        }

        Debug.Log(
            $"Current time: Day {DialogueVariableData.Instance.Get<int>("Day")}, " +
            $"{GetCurrentTimeSlotName()}"
        );
    }

    public string GetCurrentTimeSlotName()
    {
        int currentTimeSlot =
            DialogueVariableData.Instance.Get<int>("TimeSlot");

        switch (currentTimeSlot)
        {
            case 0:
                return "Morning";

            case 1:
                return "Afternoon";

            case 2:
                return "Night";

            default:
                return "Unknown";
        }
    }

    public void CompleteActivity()
    {
        // Restore normal game speed.
        Time.timeScale = 1f;

        // Consume one time slot.
        AdvanceTimeSlot();

        // Return to the Main Hub.
        SceneManager.LoadScene("mainHub");
    }
}