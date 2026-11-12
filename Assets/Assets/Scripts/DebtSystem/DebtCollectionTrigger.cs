
using DialogueSystem.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebtCollectionTrigger : MonoBehaviour
{
    private void Start()
    {
        // Find the persistent DebtManager.
        DebtManager debtManager =
            FindAnyObjectByType<DebtManager>();

        if (debtManager == null)
        {
            Debug.LogError("DebtManager was not found!");
            return;
        }

        // Check whether a collection visit is due.
        debtManager.CheckForCollectionVisit();

        // Check whether the collector should appear.
        bool collectorPending =
            DialogueVariableData.Instance.Get<bool>(
                "DebtCollectorPending"
            );

        if (collectorPending)
        {
            Debug.Log("Debt Collector is arriving!");

            SceneManager.LoadScene("debtCollect");
        }
    }
}
