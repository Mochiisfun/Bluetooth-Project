using DialogueSystem.Data;
using TMPro;
using UnityEngine;

public class RelationshipUI : MonoBehaviour
{
    [Header("character1")]
    [SerializeField] private TMP_Text VtuberAffectionValue;
    [SerializeField] private TMP_Text VtuberSuspicionValue;

    [Header("Character 2")]
    [SerializeField] private TMP_Text OLAffectionValue;
    [SerializeField] private TMP_Text OLSuspicionValue;

    [Header("Character 3")]
    [SerializeField] private TMP_Text CEOAffectionValue;
    [SerializeField] private TMP_Text CEOSuspicionValue;

    private void Start()
    {
        UpdateRelationshipUI();
    }

    private void OnEnable()
    {
        DialogueVariableData.Instance.OnVariableChanged += UpdateRelationshipUI;
    }
    private void OnDisable()
    {
        DialogueVariableData.Instance.OnVariableChanged -= UpdateRelationshipUI;
    }

    private void UpdateRelationshipUI()
    {
        VtuberAffectionValue.text =
            $"{DialogueVariableData.Instance.Get<int>("VtuberAffection")}";

        VtuberSuspicionValue.text =
            $"{DialogueVariableData.Instance.Get<int>("VtuberSuspicion")}";

        OLAffectionValue.text =
            $"{DialogueVariableData.Instance.Get<int>("OLAffection")}";

        OLSuspicionValue.text =
            $"{DialogueVariableData.Instance.Get<int>("OLSuspicion")}";

        CEOAffectionValue.text =
            $"{DialogueVariableData.Instance.Get<int>("CEOAffection")}";

        CEOSuspicionValue.text =
            $"{DialogueVariableData.Instance.Get<int>("CEOSuspicion")}";
    }
}