using JetBrains.Annotations;
using DialogueSystem.Runtime;

namespace DialogueSystem.Runtime.Narration
{
    public class DialogueOption
    {
        public string Text { get; }
        [CanBeNull] public NarrativeNode TargetNarrative { get;}
        public bool HasAlreadyBeenChosen { get; set; }
        public DialogueCondition Condition { get; }

        public DialogueOption(string text, NarrativeNode targetNode, DialogueCondition condition)
        {
            Text = text;
            TargetNarrative = targetNode;
            Condition = condition;
        }
    }
}