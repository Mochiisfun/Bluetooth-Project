using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Command;
using DialogueSystem.Runtime.Narration;
using DialogueSystem.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.Runtime.UI
{
    public class ConcreteNarrativeUI : NarrativeUI
    {
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text messageTextContainer;
        
        [Space, Header("UI Buttons")] 
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private Button disabledButtonPrefab;
        [SerializeField] private Transform buttonsParent;
        [SerializeField] private List<RectTransform> optionButtonPositions;
        [SerializeField] private Button nextMessageButton;

        [Space, Header("Dialogue Log")]
        [SerializeField] private Button logButton;
        [SerializeField] private DialogueLogUI dialogueLogUI;

        [Space, Header("UI Rendering")]
        [SerializeField] private Optional<Image> characterSprite;
        [SerializeField] private Image dialogueBubble;
    
        private List<Button> _currentOptionButtonList;
        private bool _textTyperNotNull;
        private readonly DialogueLog _dialogueLog = new();

        public IReadOnlyList<DialogueLogEntry> LogEntries => _dialogueLog.Entries;

        private string _currentSpeakerName;
    
        private void Awake()
        {
            _currentOptionButtonList = new List<Button>();
            _textTyperNotNull = textTyper != null;
            SetUIActive(false);
        }
        
        public override void DisplayOptions(
            List<DialogueOption> options,
            bool disableChosenOptions,
            ChoosePathDelegate choosePathFunction)
        {
            DisableNextNarrationUI();

            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                var optionIndex = i;

                var isDisabledOption =
                    disableChosenOptions && option.HasAlreadyBeenChosen;

                var newOptionButton = ButtonFactory.CreateButton(
                    buttonPrefab,
                    disabledButtonPrefab,
                    buttonsParent,
                    isDisabledOption,
                    DialogueCommandParser.ReplaceVariableTagsByValue(option.Text),
                    delegate
                    {
                        choosePathFunction(optionIndex);
                        RemoveOptions();
                        EnableNextNarrationUI();
                    });

                var buttonRect = newOptionButton.GetComponent<RectTransform>();

                if (optionIndex < optionButtonPositions.Count)
                {
                    ButtonFactory.PlaceButton(
                        buttonRect,
                        optionButtonPositions[optionIndex]
                    );
                }
                else
                {
                    Debug.LogWarning(
                        $"Not enough option button positions. " +
                        $"Option {optionIndex} does not have a position assigned."
                    );
                }

                _currentOptionButtonList.Add(newOptionButton);
            }
        }

        private void RemoveOptions()
        {
            _currentOptionButtonList.ForEach(button => Destroy(button.gameObject));
            _currentOptionButtonList.Clear();
        }

        private void DisplayCharacterName(string speakerName, bool hide)
        {
            speakerNameText.text = speakerName;
            speakerNameText.maxVisibleCharacters = hide ? 0 : int.MaxValue;
        }
    
        public override void DisplayDialogueBubble(DialogueMessage messageData, CharacterData characterData)
        {
            _currentSpeakerName = messageData.CharacterName;
            
            DisplayCharacterName(messageData.CharacterName, messageData.HideCharacter);
            DisplayCharacter(characterData.DefaultState.CharacterFace, messageData.HideCharacter);
        }

        public override void DisplayMessage(string text)
        {
            _dialogueLog.Add(_currentSpeakerName, text);

            messageTextContainer.text = text;
            if (textTyper)
            {
                textTyper.TypeText(text, messageTextContainer);
            }
        }

        public void DisplayDialogueLog()
        {
            dialogueLogUI.DisplayLog(LogEntries);
            dialogueLogUI.gameObject.SetActive(true);
        }

        public override void DisplayCharacter(Optional<Sprite> sprite, bool hideCharacter)
        {
            if (!characterSprite.Enabled)
            {
                return;
            }

            characterSprite.Value.sprite = sprite.Value;
            characterSprite.Value.gameObject.SetActive(!hideCharacter && sprite.Enabled);
        }

        private void EnableNextNarrationUI() => nextMessageButton.gameObject.SetActive(true);
        private void DisableNextNarrationUI() => nextMessageButton.gameObject.SetActive(false);
        
        public override void SetUIActive(bool active)
        {
            speakerNameText.gameObject.SetActive(active);
            messageTextContainer.gameObject.SetActive(active);
            buttonsParent.gameObject.SetActive(active);
            nextMessageButton.gameObject.SetActive(active);
            logButton.gameObject.SetActive(active);
            dialogueBubble.gameObject.SetActive(active);
            characterSprite.Value.gameObject.SetActive(active);
        }
    
        public override void InitializeUI()
        {
            _currentOptionButtonList = new List<Button>();
            _textTyperNotNull = textTyper != null;
            messageTextContainer.text = string.Empty;
            speakerNameText.text = string.Empty;
            DisplayCharacter(new Optional<Sprite>(), true);
        }

        public override bool IsMessageDisplaying() => _textTyperNotNull && textTyper.IsTyping;

        public override void DisplayAllMessage()
        {
            textTyper.FinishTyping();
            messageTextContainer.maxVisibleCharacters = int.MaxValue;
        }
    }
}
