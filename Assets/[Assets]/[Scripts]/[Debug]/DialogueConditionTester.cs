using DialogueSystem.Runtime;
using UnityEngine;

public class DialogueConditionTester : MonoBehaviour
{
    [SerializeField] private DialogueCondition condition;

    private void Start()
    {
        Debug.Log($"Condition result: {condition.IsMet()}");
    }
}