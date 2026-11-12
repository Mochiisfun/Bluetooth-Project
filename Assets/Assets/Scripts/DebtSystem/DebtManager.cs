
using DialogueSystem.Data;
using UnityEngine;
using UnityEngine.Events;

public class DebtManager : MonoBehaviour
{
    [Header("Collection Settings")]
    [SerializeField, Min(1)]
    private int collectionIntervalDays = 7;

    [Header("Weekly Payment Schedule")]
    [SerializeField]
    private int[] weeklyPaymentPercentages = { 5, 10, 15, 20, 50 };

    [Header("Bad Ending")]
    [SerializeField] private UnityEvent onDebtBadEnd = new UnityEvent();

    public int CollectionIntervalDays => collectionIntervalDays;

    // Get the percentage required for the current collection week.
    public int GetCurrentInstallmentPercentage()
    {
        int currentWeek =
            DialogueVariableData.Instance.Get<int>("CurrentCollectionWeek");

        if (currentWeek < 1 ||
            currentWeek > weeklyPaymentPercentages.Length)
        {
            return 0;
        }

        return weeklyPaymentPercentages[currentWeek - 1];
    }

    // Calculate the installment amount for the current week.
    public int GetCurrentInstallmentAmount()
    {
        int originalDebt =
            DialogueVariableData.Instance.Get<int>("OriginalDebt");

        int currentWeek =
            DialogueVariableData.Instance.Get<int>("CurrentCollectionWeek");

        if (currentWeek < 1 ||
            currentWeek > weeklyPaymentPercentages.Length)
        {
            return 0;
        }

        int previousInstallmentsTotal = 0;

        for (int i = 0; i < weeklyPaymentPercentages.Length; i++)
        {
            int installmentAmount;

            // The final installment absorbs any rounding difference.
            // This ensures all five installments equal the original debt.
            if (i == weeklyPaymentPercentages.Length - 1)
            {
                installmentAmount =
                    originalDebt - previousInstallmentsTotal;
            }
            else
            {
                installmentAmount = Mathf.RoundToInt(
                    originalDebt * (weeklyPaymentPercentages[i] / 100f)
                );
            }

            if (i == currentWeek - 1)
            {
                return Mathf.Max(0, installmentAmount);
            }

            previousInstallmentsTotal += installmentAmount;
        }

        return 0;
    }

    // Calculate the total amount demanded, including missed payments.
    public int GetTotalPaymentDue()
    {
        int currentInstallment = GetCurrentInstallmentAmount();

        int arrears =
            DialogueVariableData.Instance.Get<int>("DebtPaymentArrears");

        return currentInstallment + arrears;
    }


    public void AttemptDebtPayment()
    {
        // Calculate how much the collector demands.
        int paymentDue = GetTotalPaymentDue();

        // Read the player's current money.
        int playerMoney =
            DialogueVariableData.Instance.Get<int>("Money");

        // Reset the result before checking.
        DialogueVariableData.Instance.Set(
            "DebtPaymentSuccessful",
            false
        );

        // Make sure there is a valid payment due.
        if (paymentDue <= 0)
        {
            Debug.LogWarning("There is no debt payment due.");
            return;
        }

        // Check whether the player has enough money.
        if (playerMoney < paymentDue)
        {
            Debug.LogWarning(
                $"Insufficient money! Required: {paymentDue}, " +
                $"Available: {playerMoney}"
            );

            return;
        }

        // Deduct the exact payment amount.
        DialogueVariableData.Instance.Set(
            "Money",
            playerMoney - paymentDue
        );

        // Reduce the remaining debt.
        int debtRemaining =
            DialogueVariableData.Instance.Get<int>("DebtRemaining");

        DialogueVariableData.Instance.Set(
            "DebtRemaining",
            Mathf.Max(0, debtRemaining - paymentDue)
        );

        // Clear all previously missed installments.
        DialogueVariableData.Instance.Set(
            "DebtPaymentArrears",
            0
        );

        // Mark the payment as successful.
        DialogueVariableData.Instance.Set(
            "DebtPaymentSuccessful",
            true
        );

        // Move to the next collection week.
        int currentWeek =
            DialogueVariableData.Instance.Get<int>("CurrentCollectionWeek");

        DialogueVariableData.Instance.Set(
            "CurrentCollectionWeek",
            currentWeek + 1
        );

        // Schedule the next collection visit.
        int nextCollectionDay =
            DialogueVariableData.Instance.Get<int>("NextDebtCollectionDay");

        DialogueVariableData.Instance.Set(
            "NextDebtCollectionDay",
            nextCollectionDay + collectionIntervalDays
        );

        // The current collection has been resolved.
        DialogueVariableData.Instance.Set(
            "DebtCollectorPending",
            false
        );

        Debug.Log(
            $"Debt payment successful! Paid: {paymentDue}, " +
            $"Money remaining: {playerMoney - paymentDue}, " +
            $"Debt remaining: {Mathf.Max(0, debtRemaining - paymentDue)}"
        );
    }


    public void RegisterMissedPayment()
    {
        // The player did not successfully pay.
        DialogueVariableData.Instance.Set(
            "DebtPaymentSuccessful",
            false
        );

        // Get the current installment and existing unpaid amount.
        int currentInstallment = GetCurrentInstallmentAmount();

        int currentArrears =
            DialogueVariableData.Instance.Get<int>("DebtPaymentArrears");

        // Prevent registering a missed payment when nothing is due.
        if (currentInstallment <= 0 && currentArrears <= 0)
        {
            Debug.LogWarning("There is no payment to miss.");
            return;
        }

        // Add this week's installment to the unpaid balance.
        int newArrears = currentArrears + currentInstallment;

        DialogueVariableData.Instance.Set(
            "DebtPaymentArrears",
            newArrears
        );

        // Increase the total number of missed payments.
        int missedPaymentCount =
            DialogueVariableData.Instance.Get<int>("MissedPaymentCount");

        missedPaymentCount++;

        DialogueVariableData.Instance.Set(
            "MissedPaymentCount",
            missedPaymentCount
        );

        // The current collection visit has been resolved.
        DialogueVariableData.Instance.Set(
            "DebtCollectorPending",
            false
        );

        Debug.LogWarning(
            $"Missed debt payment! " +
            $"Missed payments: {missedPaymentCount}/2. " +
            $"Unpaid amount: {newArrears}"
        );

        // Two missed payments trigger the Bad Ending.
        if (missedPaymentCount >= 2)
        {
            Debug.LogError("BAD END: Player missed two debt payments!");

            Time.timeScale = 0f;

            onDebtBadEnd.Invoke();

            return;
        }

        // First missed payment: move to the next collection week.
        int currentWeek =
            DialogueVariableData.Instance.Get<int>("CurrentCollectionWeek");

        DialogueVariableData.Instance.Set(
            "CurrentCollectionWeek",
            currentWeek + 1
        );

        // Schedule the next collection visit.
        int nextCollectionDay =
            DialogueVariableData.Instance.Get<int>("NextDebtCollectionDay");

        DialogueVariableData.Instance.Set(
            "NextDebtCollectionDay",
            nextCollectionDay + collectionIntervalDays
        );

        Debug.Log(
            "First missed payment recorded. " +
            "The unpaid installment will carry over to the next visit."
        );
    }

    public void CheckForCollectionVisit()
    {
        int currentDay =
            DialogueVariableData.Instance.Get<int>("Day");

        int nextCollectionDay =
            DialogueVariableData.Instance.Get<int>("NextDebtCollectionDay");

        bool collectorPending =
            DialogueVariableData.Instance.Get<bool>("DebtCollectorPending");

        int debtRemaining =
            DialogueVariableData.Instance.Get<int>("DebtRemaining");

        bool rescueCompleted =
            DialogueVariableData.Instance.Get<bool>("DebtRescueCompleted");

        // Don't schedule another visit if one is already pending.
        if (collectorPending)
        {
            return;
        }

        // No collection is needed if the debt is settled.
        if (debtRemaining <= 0 || rescueCompleted)
        {
            return;
        }

        // Check whether the collection day has arrived.
        if (currentDay >= nextCollectionDay)
        {
            DialogueVariableData.Instance.Set(
                "DebtCollectorPending",
                true
            );

            Debug.Log(
                $"Debt Collector is due! Current Day: {currentDay}"
            );
        }
    }



    private void OnValidate()
    {
        if (weeklyPaymentPercentages == null ||
            weeklyPaymentPercentages.Length == 0)
        {
            return;
        }

        int totalPercentage = 0;

        foreach (int percentage in weeklyPaymentPercentages)
        {
            totalPercentage += percentage;
        }

        if (totalPercentage != 100)
        {
            Debug.LogWarning(
                $"Debt payment percentages currently total {totalPercentage}%. " +
                "They should add up to 100%."
            );
        }
    }
}
