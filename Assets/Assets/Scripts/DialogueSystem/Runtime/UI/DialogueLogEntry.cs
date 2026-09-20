using System;

namespace DialogueSystem.Runtime.UI
{
    [Serializable]
    public class DialogueLogEntry
    {
        public string SpeakerName { get; }
        public string Message { get; }

        public DialogueLogEntry(string speakerName, string message)
        {
            SpeakerName = speakerName;
            Message = message;
        }
    }
}