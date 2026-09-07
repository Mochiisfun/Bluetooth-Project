using DialogueSystem.Data;

namespace DialogueSystem.Runtime.Command
{
    public class VariableAddCommand : DialogueCommand
    {
        private readonly string _variableName;
        private readonly float _amount;

        public VariableAddCommand(
            int startPosition,
            bool mustExecute,
            string variableName,
            float amount)
            : base(startPosition, mustExecute)
        {
            _variableName = variableName;
            _amount = amount;
        }

        public override void Execute()
        {
            var variableType =
                DialogueVariableData.Instance.GetVariableType(_variableName);

            if (variableType == typeof(int))
            {
                DialogueVariableData.Instance.Add(
                    _variableName,
                    (int)_amount
                );

                return;
            }

            if (variableType == typeof(float))
            {
                DialogueVariableData.Instance.Add(
                    _variableName,
                    _amount
                );

                return;
            }

            UnityEngine.Debug.LogWarning(
                $"Variable '{_variableName}' cannot be modified with Add()."
            );
        }
    }
}