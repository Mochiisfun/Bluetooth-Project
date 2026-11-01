using DialogueSystem.Data;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text timeSlotText;

    private void Start()
    {
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        int currentDay =
            DialogueVariableData.Instance.Get<int>("Day");

        int currentTimeSlot =
            DialogueVariableData.Instance.Get<int>("TimeSlot");

        // Update Day display.
        dayText.text = $"Day {currentDay}";

        // Update Time Slot display.
        switch (currentTimeSlot)
        {
            case 0:
                timeSlotText.text = "Morning";
                break;

            case 1:
                timeSlotText.text = "Afternoon";
                break;

            case 2:
                timeSlotText.text = "Night";
                break;

            default:
                timeSlotText.text = "Unknown";
                break;
        }
    }
}