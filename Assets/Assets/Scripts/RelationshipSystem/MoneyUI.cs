using DialogueSystem.Data;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;

    private void Start()
    {
        UpdateMoneyUI();
    }

    private void OnEnable()
    {
        DialogueVariableData.Instance.OnVariableChanged += UpdateMoneyUI;
    }

    private void OnDisable()
    {
        DialogueVariableData.Instance.OnVariableChanged -= UpdateMoneyUI;
    }

    private void UpdateMoneyUI()
    {
        int money =
            DialogueVariableData.Instance.Get<int>("Money");

        moneyText.text = $"Money: {money}";
    }
}