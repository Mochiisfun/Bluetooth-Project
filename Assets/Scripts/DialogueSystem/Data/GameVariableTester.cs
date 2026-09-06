using UnityEngine;
using DialogueSystem.Data;

public class GameVariableTester : MonoBehaviour
{
    private void Start()
    {
        // Create a test variable
        if (!DialogueVariableData.Instance.Exists("TestMoney"))
        {
            DialogueVariableData.Instance.Create("TestMoney", 100);
        }

        // Read it
        int money = DialogueVariableData.Instance.Get<int>("TestMoney");

        Debug.Log($"Starting money: {money}");

        // Add to it
        DialogueVariableData.Instance.Add("TestMoney", 50);

        // Read it again
        money = DialogueVariableData.Instance.Get<int>("TestMoney");

        Debug.Log($"After adding 50: {money}");

        // Set it directly
        DialogueVariableData.Instance.Set("TestMoney", 500);

        // Read it one more time
        money = DialogueVariableData.Instance.Get<int>("TestMoney");

        Debug.Log($"After setting to 500: {money}");
    }
}