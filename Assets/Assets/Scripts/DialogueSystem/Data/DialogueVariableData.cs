using System;
using System.Collections.Generic;
using System.Globalization;
using DialogueSystem.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Data
{
    [Serializable]
    
    public abstract class Variable
    {
        [field: SerializeField] public string Name { get; set; }
        public abstract object GetValue();
        public abstract void SetValue(object newValue);
        public abstract override string ToString();
    }

    [Serializable]
    public class IntVariable : Variable
    {
        [SerializeField] internal int intValue;

        [SerializeField] private bool useClamp;
        [SerializeField] private int minValue;
        [SerializeField] private int maxValue;
        
        public override object GetValue() => intValue;

        public override void SetValue(object newValue)
        {
            try
            {
                int parsedValue = int.Parse(newValue.ToString());

                if (useClamp)
                {
                    parsedValue = Mathf.Clamp(
                        parsedValue,
                        minValue,
                        maxValue
                    );
                }

                intValue = parsedValue;
            }
            catch (Exception e)
            {
                LogHandler.Alert($"Error while parsing int value: {e.Message}");
            }
        }

        public void ConfigureClamp(bool shouldClamp, int min, int max)
        {
            useClamp = shouldClamp;
            minValue = min;
            maxValue = max;

            if (useClamp)
            {
                intValue = Mathf.Clamp(intValue, minValue, maxValue);
            }
        }

        public override string ToString() => intValue.ToString(CultureInfo.InvariantCulture);
    }

    [Serializable]
    public class StringVariable : Variable
    {
        [SerializeField] internal string stringValue;

        public override object GetValue() => stringValue;
        
        public override void SetValue(object newValue)
        {
            try
            {
                stringValue = newValue.ToString();
            }
            catch (Exception e)
            {
                LogHandler.Alert($"Error while parsing string value: {e.Message}");
            }
        }
        
        public override string ToString() => stringValue;
    }

    [Serializable]
    public class FloatVariable : Variable
    {
        [SerializeField] internal float floatValue;

        public override object GetValue() => floatValue;
        
        public override void SetValue(object newValue)
        {
            try
            {
                floatValue = float.Parse(newValue.ToString());
            }
            catch (Exception e)
            {
                LogHandler.Alert($"Error while parsing float value: {e.Message}");
            }
        }
        
        public override string ToString() => floatValue.ToString(CultureInfo.InvariantCulture);
    }

    [Serializable]
        public class BoolVariable : Variable
        {
            [SerializeField] internal bool boolValue;

            public override object GetValue() => boolValue;

            public override void SetValue(object newValue)
            {
                try
                {
                    boolValue = bool.Parse(newValue.ToString());
                }
                catch (Exception e)
             {
                  LogHandler.Alert($"Error while parsing bool value: {e.Message}");
                }
            }

            public override string ToString() => boolValue.ToString();
        }


    [CreateAssetMenu(fileName = "DialogueVariableData", menuName = "ScriptableObjects/EasyScriptableSingletons/DialogueVariableData")]
    
    public sealed class DialogueVariableData : EasyScriptableSingleton<DialogueVariableData>
    {
        public event Action OnVariableChanged;

        protected override string PathToResources => "Assets/Resources";
        protected override string ResourcesPath => "Dialogue System Data/Dialogue";
        protected override string FileName => "DialogueVariableData";
        
        [SerializeReference] private List<Variable> variables;

        protected override void Initialize() => variables = new List<Variable>();

        public string GetValueAsString(string variableName)
        {
            var variable = variables.Find(v => v.Name == variableName);
            return variable?.ToString();
        }

        public void AddDialogueVariable<T>(string variableName, T value)
        {
            var valueToString = value.ToString();
            if (variables.Exists(v => v.Name == variableName))
            {
                ChangeDialogueVariable(variableName, valueToString);
                return;
            }
            
            switch (value)
            {
                case int _:
                    variables.Add(new IntVariable {Name = variableName, intValue = (int) (object) value});
                    break;
                case string _:
                    variables.Add(new StringVariable {Name = variableName, stringValue = (string) (object) value});
                    break;
                case float _:
                    variables.Add(new FloatVariable {Name = variableName, floatValue = (float) (object) value});
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            
            SaveRuntimeData();
        }

        public void ChangeDialogueVariable<T>(string variableName, T newValue)
        {
            var variable = variables.Find(v => v.Name == variableName);
            if (variable != null)
            {
                variable.SetValue(newValue);
            }
            else
            {
                AddDialogueVariable(variableName, newValue);
            }
            SaveRuntimeData();
        }

        public void RemoveAllDialogueVariables()
        {
            variables.Clear();
            SaveRuntimeData();
        }
        
        public void RemoveDialogueVariable(string variableName)
        {
            variables.RemoveAll(v => v.Name == variableName);
            SaveRuntimeData();
        }
        
        public void ListAllDialogueVariables()
        {
            LogHandler.Log("Dialogue Variables:", LogHandler.Color.Green);
            
            if(variables.Count == 0)
            {
                LogHandler.Log("No variables.", LogHandler.Color.Green);
                return;
            }
            
            foreach (var variable in variables)
            {
                LogHandler.Log( $"{variable.Name}: {variable}", LogHandler.Color.Green);
            }
        }

        //added code so you can get value not as a
        public T GetValue<T>(string variableName)
        {
            var variable = variables.Find(v => v.Name == variableName);

            if (variable == null)
            {
                Debug.LogWarning(
                    $"Dialogue variable '{variableName}' does not exist."
                );

                return default;
            }

            if (variable.GetValue() is T value)
            {
                return value;
            }

            Debug.LogWarning(
                $"Dialogue variable '{variableName}' is not of type {typeof(T).Name}."
            );

            return default;
        }





        public void Create<T>(string variableName, T value)
        {
            if (variables.Exists(v => v.Name == variableName))
            {
                Debug.LogWarning(
                    $"Game variable '{variableName}' already exists."
                );

                return;
            }

            switch (value)
            {
                case int intValue:
                    variables.Add(new IntVariable
                    {
                        Name = variableName,
                        intValue = intValue
                    });
                    break;

                case string stringValue:
                    variables.Add(new StringVariable
                    {
                        Name = variableName,
                        stringValue = stringValue
                    });
                    break;

                case float floatValue:
                    variables.Add(new FloatVariable
                    {
                        Name = variableName,
                        floatValue = floatValue
                    });
                    break;

                case bool boolValue:
                    variables.Add(new BoolVariable
                    {
                        Name = variableName,
                        boolValue = boolValue
                    });
                break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "Only int, float, string and bool variables are supported."
                  );
            }

                SaveRuntimeData();
        }

        public Type GetVariableType(string variableName)
        {
            var variable = variables.Find(v => v.Name == variableName);

            if (variable == null)
            {
                return null;
            }

            return variable.GetValue()?.GetType();
        }



        public T Get<T>(string variableName)
        {
            var variable = variables.Find(v => v.Name == variableName);

            if (variable == null)
            {
                Debug.LogWarning(
                    $"Game variable '{variableName}' does not exist."
                );

                return default;
            }

            if (variable.GetValue() is T value)
            {
                return value;
            }

            Debug.LogWarning(
                $"Game variable '{variableName}' is not of type {typeof(T).Name}."
            );

            return default;
        }


        public void Set<T>(string variableName, T newValue)
        {
            var variable = variables.Find(v => v.Name == variableName);

            if (variable == null)
            {
                Debug.LogWarning(
                    $"Cannot set game variable '{variableName}' because it does not exist."
                );

                return;
            }

            variable.SetValue(newValue);

            SaveRuntimeData();

            OnVariableChanged?.Invoke();
        }


        public void Add<T>(string variableName, T amount)
        {
            var variable = variables.Find(v => v.Name == variableName);

            if (variable == null)
            {
                Debug.LogWarning(
                    $"Cannot add to game variable '{variableName}' because it does not exist."
                );

                return;
            }

            var currentValue = variable.GetValue();

            switch (currentValue)
            {
                case int currentInt when amount is int intAmount:
                    variable.SetValue(currentInt + intAmount);
                    break;

                case float currentFloat when amount is float floatAmount:
                    variable.SetValue(currentFloat + floatAmount);
                    break;

               default:
                    Debug.LogWarning(
                        $"Game variable '{variableName}' does not support Add()."
                    );
                    return;
            }

            SaveRuntimeData();
            
            OnVariableChanged?.Invoke();
        }

        public void ConfigureIntClamp(
            string variableName,
            bool useClamp,
            int minValue,
            int maxValue)
        {
            var variable = variables.Find(v => v.Name == variableName);

            if (variable is not IntVariable intVariable)
            {
                Debug.LogWarning(
                    $"Cannot configure clamp for '{variableName}' because it is not an int variable."
                );

                return;
            }

            intVariable.ConfigureClamp(
                useClamp,
                minValue,
                maxValue
            );

            SaveRuntimeData();
        }

        public bool Exists(string variableName)
        {
            return variables.Find(v => v.Name == variableName) != null;
        }
    }
    
}