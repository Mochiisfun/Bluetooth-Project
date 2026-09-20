using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogueSystem.Runtime.UI
{
    public class ButtonFactory : MonoBehaviour
    {
        public static Button CreateButton(Button buttonPrefab, Button disabledButtonPrefab, Transform parent, bool isDisabled, string buttonText, UnityAction onClickAction)
        {
            var newButton = Instantiate(isDisabled ? disabledButtonPrefab : buttonPrefab, parent);
            newButton.interactable = !isDisabled;
            
            var optionTextContainer = newButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            optionTextContainer.text = buttonText;

            newButton.onClick.AddListener(onClickAction);

            return newButton;
        }
        
        public static void PlaceButton(RectTransform button,RectTransform position)
        {
            button.anchoredPosition = position.anchoredPosition;
        }
    }
}