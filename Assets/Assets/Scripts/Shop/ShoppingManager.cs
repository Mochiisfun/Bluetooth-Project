using UnityEngine;

public class ShoppingManager : MonoBehaviour
{
    public void CompleteShopping()
    {
        // Find the persistent TimeManager.
        TimeManager timeManager = FindAnyObjectByType<TimeManager>();

        if (timeManager == null)
        {
            Debug.LogError(
                "Cannot complete shopping: TimeManager was not found!"
            );

            return;
        }

        // Consume one time slot and return to Main Hub.
        timeManager.CompleteActivity();
    }
}