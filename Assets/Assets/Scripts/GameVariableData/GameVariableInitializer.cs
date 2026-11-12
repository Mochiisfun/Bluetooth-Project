
using System;
using System.Collections.Generic;
using System.IO;
using DialogueSystem.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Serializable]
    private class SaveFileData
    {
        public List<SavedVariable> variables = new();
    }

    [Serializable]
    private class SavedVariable
    {
        public string variableName;
        public VariableType variableType;

        public int intValue;
        public float floatValue;
        public string stringValue;
        public bool boolValue;
    }

    private static GameVariableInitializer instance;

    private string SaveFilePath =>
        Path.Combine(Application.persistentDataPath, "savefile.json");

    [Header("Starting Game Variables")]
    [SerializeField] private List<StartingVariable> startingVariables = new();

    private void Awake()
    {
        // Keep only one variable manager across all scenes.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Create missing variables with their starting values first.
        InitializeVariables();

        // Then overwrite those values with saved progress, if available.
        LoadGame();
    }
    public static void SaveCurrentGame()
    {
        if (instance != null)
        {
            instance.SaveGame();
        }
        else
        {
            Debug.LogWarning(
                "Cannot save: GameVariableInitializer has not been initialized."
            );
        }
    }

    public static void LoadCurrentGame()
    {
        if (instance != null)
        {
            instance.LoadGame();
        }
        else
        {
            Debug.LogWarning(
                "Cannot load: GameVariableInitializer has not been initialized."
            );
        }
    }

    public void SaveGame()
    {
        try
        {
            SaveFileData saveData = new SaveFileData();

            foreach (StartingVariable variable in startingVariables)
            {
                if (string.IsNullOrWhiteSpace(variable.VariableName))
                {
                    continue;
                }

                if (!DialogueVariableData.Instance.Exists(variable.VariableName))
                {
                    continue;
                }

                if (!HasExpectedType(variable))
                {
                    Debug.LogWarning(
                        $"Skipping save for '{variable.VariableName}' because its type does not match."
                    );
                    continue;
                }

                SavedVariable saved = new SavedVariable
                {
                    variableName = variable.VariableName,
                    variableType = variable.Type
                };

                switch (variable.Type)
                {
                    case VariableType.Int:
                        saved.intValue =
                            DialogueVariableData.Instance.Get<int>(variable.VariableName);
                        break;

                    case VariableType.Float:
                        saved.floatValue =
                            DialogueVariableData.Instance.Get<float>(variable.VariableName);
                        break;

                    case VariableType.String:
                        saved.stringValue =
                            DialogueVariableData.Instance.Get<string>(variable.VariableName);
                        break;

                    case VariableType.Bool:
                        saved.boolValue =
                            DialogueVariableData.Instance.Get<bool>(variable.VariableName);
                        break;
                }

                saveData.variables.Add(saved);
            }

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SaveFilePath, json);

            Debug.Log($"Game saved successfully: {SaveFilePath}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to save game: {exception.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.Log("No save file found. Using starting game variables.");
            return;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveFileData saveData = JsonUtility.FromJson<SaveFileData>(json);

            if (saveData == null || saveData.variables == null)
            {
                Debug.LogWarning("The save file is empty or invalid.");
                return;
            }

            foreach (StartingVariable variable in startingVariables)
            {
                if (string.IsNullOrWhiteSpace(variable.VariableName))
                {
                    continue;
                }

                if (!DialogueVariableData.Instance.Exists(variable.VariableName))
                {
                    continue;
                }

                SavedVariable saved = saveData.variables.Find(
                    entry => entry.variableName == variable.VariableName
                );

                if (saved == null)
                {
                    continue;
                }

                if (saved.variableType != variable.Type ||
                    !HasExpectedType(variable))
                {
                    Debug.LogWarning(
                        $"Skipping saved value for '{variable.VariableName}' because its type does not match."
                    );
                    continue;
                }

                switch (variable.Type)
                {
                    case VariableType.Int:
                        DialogueVariableData.Instance.Set(
                            variable.VariableName, saved.intValue);
                        break;

                    case VariableType.Float:
                        DialogueVariableData.Instance.Set(
                            variable.VariableName, saved.floatValue);
                        break;

                    case VariableType.String:
                        DialogueVariableData.Instance.Set(
                            variable.VariableName, saved.stringValue);
                        break;

                    case VariableType.Bool:
                        DialogueVariableData.Instance.Set(
                            variable.VariableName, saved.boolValue);
                        break;
                }
            }

            Debug.Log("Game loaded successfully. Reloading mainHub.");
            SceneManager.LoadScene("mainHub");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to load game: {exception.Message}");
        }
    }

    private void InitializeVariables()
    {
        foreach (StartingVariable variable in startingVariables)
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
                    variable.VariableName, variable.IntValue);
                break;

            case VariableType.Float:
                DialogueVariableData.Instance.Create(
                    variable.VariableName, variable.FloatValue);
                break;

            case VariableType.String:
                DialogueVariableData.Instance.Create(
                    variable.VariableName, variable.StringValue);
                break;

            case VariableType.Bool:
                DialogueVariableData.Instance.Create(
                    variable.VariableName, variable.BoolValue);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckVariableType(StartingVariable variable)
    {
        Type existingType =
            DialogueVariableData.Instance.GetVariableType(variable.VariableName);

        Type expectedType = GetExpectedSystemType(variable.Type);

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

    private Type GetExpectedSystemType(VariableType type)
    {
        return type switch
        {
            VariableType.Int => typeof(int),
            VariableType.Float => typeof(float),
            VariableType.String => typeof(string),
            VariableType.Bool => typeof(bool),
            _ => null
        };
    }

    private bool HasExpectedType(StartingVariable variable)
    {
        Type actualType =
            DialogueVariableData.Instance.GetVariableType(variable.VariableName);

        return actualType == GetExpectedSystemType(variable.Type);
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
