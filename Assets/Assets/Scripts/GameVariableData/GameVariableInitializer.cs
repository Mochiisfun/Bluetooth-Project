using System;
using System.Collections.Generic;
using DialogueSystem.Data;
using UnityEngine;

public class GameVariableInitializer : MonoBehaviour
{
    public enum VariableType
    {
        Int,
        Float,
        String,
        Bool
    }

    [Serializable]
    public class StartingVariable
    {
        [SerializeField] private string variableName;
        [SerializeField] private VariableType variableType;

        [Header("Value")]
        [SerializeField] private int intValue;
        [SerializeField] private float floatValue;
        [SerializeField] private string stringValue;
        [SerializeField] private bool boolValue;

        [Header("Optional Int Clamp")]
        [SerializeField] private bool useIntClamp;
        [SerializeField] private int minIntValue = 0;
        [SerializeField] private int maxIntValue = 100;

        public string VariableName => variableName;
        public VariableType Type => variableType;

        public int IntValue => intValue;
        public float FloatValue => floatValue;
        public string StringValue => stringValue;
        public bool BoolValue => boolValue;

        public bool UseIntClamp => useIntClamp;
        public int MinIntValue => minIntValue;
        public int MaxIntValue => maxIntValue;
    }

    [Header("Starting Game Variables")]
    [SerializeField] private List<StartingVariable> startingVariables = new();

    private void Awake()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        foreach (var variable in startingVariables)
        {
            if (string.IsNullOrWhiteSpace(variable.VariableName))
            {
                Debug.LogWarning(
                    "Game Variable Initializer contains a variable with no name."
                );

                continue;
            }

            if (DialogueVariableData.Instance.Exists(variable.VariableName))
            {
                CheckVariableType(variable);
                ConfigureIntClamp(variable);
                continue;
            }

            CreateVariable(variable);
            ConfigureIntClamp(variable);
        }
    }

    private void CreateVariable(StartingVariable variable)
    {
        switch (variable.Type)
        {
            case VariableType.Int:
                DialogueVariableData.Instance.Create(
                    variable.VariableName,
                    variable.IntValue
                );
                break;

            case VariableType.Float:
                DialogueVariableData.Instance.Create(
                    variable.VariableName,
                    variable.FloatValue
                );
                break;

            case VariableType.String:
                DialogueVariableData.Instance.Create(
                    variable.VariableName,
                    variable.StringValue
                );
                break;

            case VariableType.Bool:
                DialogueVariableData.Instance.Create(
                    variable.VariableName,
                    variable.BoolValue
                );
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckVariableType(StartingVariable variable)
    {
        var existingType =
            DialogueVariableData.Instance.GetVariableType(variable.VariableName);

        Type expectedType = variable.Type switch
        {
            VariableType.Int => typeof(int),
            VariableType.Float => typeof(float),
            VariableType.String => typeof(string),
            VariableType.Bool => typeof(bool),
            _ => null
        };

        if (existingType != expectedType)
        {
            Debug.LogWarning(
                $"Game variable '{variable.VariableName}' already exists, " +
                $"but its type does not match the initializer. " +
                $"Expected {expectedType?.Name}, " +
                $"found {existingType?.Name}."
            );
        }
    }

    private void ConfigureIntClamp(StartingVariable variable)
    {
        if (variable.Type != VariableType.Int)
        {
            return;
        }

        DialogueVariableData.Instance.ConfigureIntClamp(
            variable.VariableName,
            variable.UseIntClamp,
            variable.MinIntValue,
            variable.MaxIntValue
        );
    }
}