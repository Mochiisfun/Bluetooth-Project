using DialogueSystem.Data;

namespace DialogueSystem.Runtime.Interaction
{
    public class DialogueLauncher : DialogueMonoBehaviour
    {
        public void PlayDialogue(DialogueContainer dialogueContainer)
        {
            narrativeScriptableObject = dialogueContainer;

            StartDialogue();
        }
    }
}