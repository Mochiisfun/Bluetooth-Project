using UnityEngine;
using DialogueSystem.Data;

public class GameVariableTester : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("===== GAME VARIABLE TEST =====");

        // Read variables created by GameVariableInitializer
        int money = DialogueVariableData.Instance.Get<int>("Money");
        int day = DialogueVariableData.Instance.Get<int>("Day");
        int mayaAffection =
            DialogueVariableData.Instance.Get<int>("MayaAffection");
        bool hasMetMaya =
            DialogueVariableData.Instance.Get<bool>("HasMetMaya");
        string playerName =
            DialogueVariableData.Instance.Get<string>("PlayerName");

        Debug.Log($"Money: {money}");
        Debug.Log($"Day: {day}");
        Debug.Log($"Maya Affection: {mayaAffection}");
        Debug.Log($"Has Met Maya: {hasMetMaya}");
        Debug.Log($"Player Name: {playerName}");

        // Test changing a variable
        DialogueVariableData.Instance.Add("Money", 50);

        money = DialogueVariableData.Instance.Get<int>("Money");

        Debug.Log($"Money after adding 50: {money}");

        // Test setting a variable
        DialogueVariableData.Instance.Set("HasMetMaya", true);

        hasMetMaya =
            DialogueVariableData.Instance.Get<bool>("HasMetMaya");

        Debug.Log($"Has Met Maya after Set: {hasMetMaya}");

        Debug.Log("==============================");
    }
}