using UnityEngine;

public class MoneyRequestSystem : MonoBehaviour
{
    public enum RequestSize
    {
        Little,
        Normal,
        Big,
        VeryBig
    }

    public enum AnswerQuality
    {
        VeryConvincing,
        Good,
        Neutral,
        Suspicious,
        ExtremelySuspicious
    }

    private int GetRequestDifficulty(RequestSize requestSize)
    {
        return requestSize switch
        {
            RequestSize.Little => 10,
            RequestSize.Normal => 30,
            RequestSize.Big => 50,
            RequestSize.VeryBig => 70,
            _ => 0
        };
    }

    private int GetAnswerBonus(AnswerQuality answerQuality)
    {
        return answerQuality switch
        {
            AnswerQuality.VeryConvincing => 30,
            AnswerQuality.Good => 15,
            AnswerQuality.Neutral => 0,
            AnswerQuality.Suspicious => -20,
            AnswerQuality.ExtremelySuspicious => -40,
            _ => 0
        };
    }
}