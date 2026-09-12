using System.Collections.Generic;

namespace DialogueSystem.Runtime.UI
{
    public class DialogueLog
    {
        private readonly List<DialogueLogEntry> _entries = new();

        public IReadOnlyList<DialogueLogEntry> Entries => _entries;

        public void Add(string speakerName, string message)
        {
            _entries.Add(
                new DialogueLogEntry(speakerName, message)
            );
        }

        public void Clear()
        {
            _entries.Clear();
        }

    }
}