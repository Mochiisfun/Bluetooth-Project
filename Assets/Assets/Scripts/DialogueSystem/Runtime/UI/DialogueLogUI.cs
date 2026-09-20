using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DialogueSystem.Runtime.UI
{
    public class DialogueLogUI : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject logEntryPrefab;

        public void DisplayLog(
            IReadOnlyList<DialogueLogEntry> entries)
        {
            ClearLog();

            foreach (var entry in entries)
            {
                var logEntry = Instantiate(
                    logEntryPrefab,
                    content
                );

                var texts = logEntry.GetComponentsInChildren<TMP_Text>();

                if (texts.Length >= 2)
                {
                    texts[0].text = entry.SpeakerName;
                    texts[1].text = entry.Message;
                }
            }
        }

        private void ClearLog()
        {
            for (var i = content.childCount - 1; i >= 0; i--)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }
}