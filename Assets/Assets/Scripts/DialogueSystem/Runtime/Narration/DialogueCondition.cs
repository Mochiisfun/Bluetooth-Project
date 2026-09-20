using System;
using System.Collections.Generic;
using DialogueSystem.Data;
using UnityEngine;

namespace DialogueSystem.Runtime
{
    [Serializable]
public class DialogueCondition
    {
        public enum ComparisonOperator
        {
            Equal,
            NotEqual,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual
        }

        [field: SerializeField]
        public string VariableName { get; set; }

        [field: SerializeField]
        public ComparisonOperator Operator { get; set; }

        [field: SerializeField]
        public string ComparisonValue { get; set; }

        [field: SerializeField]
        public bool Enabled { get; set; }

        public bool IsMet()
        {
            var variableType =
                DialogueVariableData.Instance.GetVariableType(VariableName);

            if (variableType == typeof(int))
            {
                return Compare(
                    DialogueVariableData.Instance.Get<int>(VariableName),
                    int.Parse(ComparisonValue)
                );
            }

            if (variableType == typeof(float))
            {
                return Compare(
                    DialogueVariableData.Instance.Get<float>(VariableName),
                    float.Parse(ComparisonValue)
                );
            }

            if (variableType == typeof(bool))
            {
                return Compare(
                    DialogueVariableData.Instance.Get<bool>(VariableName),
                    bool.Parse(ComparisonValue)
                );
            }

            if (variableType == typeof(string))
            {
                return Compare(
                    DialogueVariableData.Instance.Get<string>(VariableName),
                    ComparisonValue
                );
            }

            Debug.LogWarning(
                $"Cannot evaluate condition for variable '{VariableName}'."
            );

            return false;
        }

        private bool Compare<T>(T currentValue, T targetValue)
        {
            return Operator switch
            {
                ComparisonOperator.Equal =>
                    EqualityComparer<T>.Default.Equals(currentValue, targetValue),

                ComparisonOperator.NotEqual =>
                    !EqualityComparer<T>.Default.Equals(currentValue, targetValue),

                _ => CompareNumeric(currentValue, targetValue)
            };
        }

        private bool CompareNumeric<T>(T currentValue, T targetValue)
        {
            if (currentValue is not IComparable comparable)
            {
                return false;
            }

            var result = comparable.CompareTo(targetValue);

            return Operator switch
            {
                ComparisonOperator.GreaterThan => result > 0,
                ComparisonOperator.LessThan => result < 0,
                ComparisonOperator.GreaterThanOrEqual => result >= 0,
                ComparisonOperator.LessThanOrEqual => result <= 0,
                _ => false
            };
        }
    }
}