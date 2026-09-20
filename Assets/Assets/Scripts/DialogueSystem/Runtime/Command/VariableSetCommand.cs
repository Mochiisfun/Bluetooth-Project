using DialogueSystem.Data;
using UnityEngine;

namespace DialogueSystem.Runtime.Command
{
    public class VariableSetCommand : DialogueCommand
    {
        private readonly string _variableName;
        private readonly string _value;

        public VariableSetCommand(
            int startPosition,
            bool mustExecute,
            string variableName,
            string value)
            : base(startPosition, mustExecute)
        {
            _variableName = variableName;
            _value = value;
        }

        public override void Execute()
        {
            var variableType =
                DialogueVariableData.Instance.GetVariableType(_variableName);

            if (variableType == typeof(int))
            {
                if (int.TryParse(_value, out var intValue))
                {
                    DialogueVariableData.Instance.Set(
                        _variableName,
                        intValue
                    );
                }
                else
                {
                    Debug.LogWarning(
                        $"Cannot set '{_variableName}' to '{_value}' because '{_value}' is not an int."
                    );
                }

                return;
            }

            if (variableType == typeof(float))
            {
                if (float.TryParse(_value, out var floatValue))
                {
                    DialogueVariableData.Instance.Set(
                        _variableName,
                        floatValue
                    );
                }
                else
                {
                    Debug.LogWarning(
                        $"Cannot set '{_variableName}' to '{_value}' because '{_value}' is not a float."
                    );
                }

                return;
            }

            if (variableType == typeof(bool))
            {
                if (bool.TryParse(_value, out var boolValue))
                {
                    DialogueVariableData.Instance.Set(
                        _variableName,
                        boolValue
                    );
                }
                else
                {
                    Debug.LogWarning(
                        $"Cannot set '{_variableName}' to '{_value}' because '{_value}' is not a bool."
                    );
                }

                return;
            }

            if (variableType == typeof(string))
            {
                DialogueVariableData.Instance.Set(
                    _variableName,
                    _value
                );

                return;
            }

            Debug.LogWarning(
                $"Cannot set variable '{_variableName}'. Variable does not exist or has an unsupported type."
            );
        }
    }
}