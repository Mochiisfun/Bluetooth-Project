
using DialogueSystem.Data;
using DialogueSystem.Runtime.Interaction;
using UnityEngine;

public class DebtCollectionStarter : MonoBehaviour
{
    [Header("Dialogue Setup")]
    [SerializeField] private DialogueLauncher dialogueLauncher;
    [SerializeField] private DialogueContainer debtCollectionDialogue;

    private void Start()
    {
        if (dialogueLauncher == null)
        {
            Debug.LogError("DialogueLauncher is missing!");
            return;
        }

        if (debtCollectionDialogue == null)
        {
            Debug.LogError("DebtCollectionDialogue is missing!");
            return;
        }

        // Start the debt collection conversation.
        dialogueLauncher.PlayDialogue(debtCollectionDialogue);
    }
}
