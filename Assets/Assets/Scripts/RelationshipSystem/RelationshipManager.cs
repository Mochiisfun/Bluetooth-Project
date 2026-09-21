using DialogueSystem.Data;
using UnityEngine;

public class RelationshipManager : MonoBehaviour
{
    private const int MinRelationshipValue = 0;
    private const int MaxRelationshipValue = 100;

    public void SetRelationshipValue(string variableName, int value)
    {
        int clampedValue = Mathf.Clamp(
            value,
            MinRelationshipValue,
            MaxRelationshipValue
        );

        DialogueVariableData.Instance.Set(variableName, clampedValue);
    }

    public void AddRelationshipValue(string variableName, int amount)
    {
        int currentValue =
            DialogueVariableData.Instance.Get<int>(variableName);

        int newValue = currentValue + amount;

        SetRelationshipValue(variableName, newValue);
    }
}