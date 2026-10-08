using UnityEngine;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Interaction;

public class MoneyRequestSystem : MonoBehaviour
{
    [Header("Player Variables")]
    [SerializeField] private string moneyVariableName = "Money";

    [Header("Date System")]
    [SerializeField] private DateManager dateManager;

    [Header("Money Request Dialogues")]
    [SerializeField] private DialogueContainer vtuberMoneyRequestDialogue;
    [SerializeField] private DialogueContainer olMoneyRequestDialogue;
    [SerializeField] private DialogueContainer ceoMoneyRequestDialogue;

    [SerializeField] private DialogueLauncher dialogueLauncher;

    [Header("Request Amounts")]
    [SerializeField] private int littleRequestAmount = 100;
    [SerializeField] private int normalRequestAmount = 300;
    [SerializeField] private int bigRequestAmount = 600;
    [SerializeField] private int veryBigRequestAmount = 1000;

    [Header("Request Difficulties")]
    [SerializeField] private int littleRequestDifficulty = 10;
    [SerializeField] private int normalRequestDifficulty = 30;
    [SerializeField] private int bigRequestDifficulty = 50;
    [SerializeField] private int veryBigRequestDifficulty = 70;

    [Header("Answer Quality Bonuses")]
    [SerializeField] private int veryConvincingBonus = 30;
    [SerializeField] private int goodBonus = 15;
    [SerializeField] private int neutralBonus = 0;
    [SerializeField] private int suspiciousBonus = -20;
    [SerializeField] private int extremelySuspiciousBonus = -40;

    [Header("Success Suspicion Increase")]
    [SerializeField] private int littleSuccessSuspicion = 2;
    [SerializeField] private int normalSuccessSuspicion = 5;
    [SerializeField] private int bigSuccessSuspicion = 10;
    [SerializeField] private int veryBigSuccessSuspicion = 15;

    [Header("Failed Request Consequences")]
    [SerializeField] private int littleFailureSuspicion = 5;
    [SerializeField] private int normalFailureSuspicion = 10;
    [SerializeField] private int bigFailureSuspicion = 15;
    [SerializeField] private int veryBigFailureSuspicion = 20;

    [SerializeField] private int littleFailureAffectionLoss = 2;
    [SerializeField] private int normalFailureAffectionLoss = 5;
    [SerializeField] private int bigFailureAffectionLoss = 10;
    [SerializeField] private int veryBigFailureAffectionLoss = 15;

    [Header("Answer Suspicion Modifiers")]
    [SerializeField] private int veryConvincingSuspicionModifier = -5;
    [SerializeField] private int goodSuspicionModifier = -2;
    [SerializeField] private int neutralSuspicionModifier = 0;
    [SerializeField] private int suspiciousSuspicionModifier = 5;
    [SerializeField] private int extremelySuspiciousSuspicionModifier = 10;

    private TargetCharacter pendingTarget;
    private RequestSize pendingRequestSize;
    private bool hasPendingRequest;

    public enum TargetCharacter
    {
        Vtuber,
        OL,
        CEO
    }
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

    public struct RequestResult
    {
        public bool Success;
        public int Score;
        public int Amount;

        public RequestResult(bool success, int score, int amount)
        {
            Success = success;
            Score = score;
            Amount = amount;
        }
    }

    private void GetCharacterVariableNames(
        TargetCharacter targetCharacter,
        out string affectionVariable,
        out string suspicionVariable)
    {
        switch (targetCharacter)
        {
            case TargetCharacter.Vtuber:
                affectionVariable = "VtuberAffection";
                suspicionVariable = "VtuberSuspicion";
                break;

            case TargetCharacter.OL:
                affectionVariable = "OLAffection";
                suspicionVariable = "OLSuspicion";
                break;

            case TargetCharacter.CEO:
                affectionVariable = "CEOAffection";
                suspicionVariable = "CEOSuspicion";
                break;

            default:
                affectionVariable = "";
                suspicionVariable = "";
                break;
        }
    }

    private int GetRequestDifficulty(RequestSize requestSize)
    {
        return requestSize switch
        {
            RequestSize.Little => littleRequestDifficulty,
            RequestSize.Normal => normalRequestDifficulty,
            RequestSize.Big => bigRequestDifficulty,
            RequestSize.VeryBig => veryBigRequestDifficulty,
            _ => 0
        };
    }

    private int GetRequestAmount(RequestSize requestSize)
    {
        return requestSize switch
        {
            RequestSize.Little => littleRequestAmount,
            RequestSize.Normal => normalRequestAmount,
            RequestSize.Big => bigRequestAmount,
            RequestSize.VeryBig => veryBigRequestAmount,
            _ => 0
        };
    }

    private int GetAnswerBonus(AnswerQuality answerQuality)
    {
        return answerQuality switch
        {
            AnswerQuality.VeryConvincing => veryConvincingBonus,
            AnswerQuality.Good => goodBonus,
            AnswerQuality.Neutral => neutralBonus,
            AnswerQuality.Suspicious => suspiciousBonus,
            AnswerQuality.ExtremelySuspicious => extremelySuspiciousBonus,
            _ => 0
        };
    }

    private int GetSuccessSuspicionIncrease(RequestSize requestSize)
    {
        return requestSize switch
        {
            RequestSize.Little => littleSuccessSuspicion,
            RequestSize.Normal => normalSuccessSuspicion,
            RequestSize.Big => bigSuccessSuspicion,
            RequestSize.VeryBig => veryBigSuccessSuspicion,
            _ => 0
        };
    }

    private int GetFailureSuspicionIncrease(RequestSize requestSize)
    {
        return requestSize switch
        {
            RequestSize.Little => littleFailureSuspicion,
            RequestSize.Normal => normalFailureSuspicion,
            RequestSize.Big => bigFailureSuspicion,
            RequestSize.VeryBig => veryBigFailureSuspicion,
            _ => 0
        };
    }

    private int GetFailureAffectionLoss(RequestSize requestSize)
    {
        return requestSize switch
        {
            RequestSize.Little => littleFailureAffectionLoss,
            RequestSize.Normal => normalFailureAffectionLoss,
            RequestSize.Big => bigFailureAffectionLoss,
            RequestSize.VeryBig => veryBigFailureAffectionLoss,
            _ => 0
        };
    }

    private int GetAnswerSuspicionModifier(AnswerQuality answerQuality)
    {
        return answerQuality switch
        {
            AnswerQuality.VeryConvincing => veryConvincingSuspicionModifier,
            AnswerQuality.Good => goodSuspicionModifier,
            AnswerQuality.Neutral => neutralSuspicionModifier,
            AnswerQuality.Suspicious => suspiciousSuspicionModifier,
            AnswerQuality.ExtremelySuspicious => extremelySuspiciousSuspicionModifier,
            _ => 0
        };
    }

    private DialogueContainer GetMoneyRequestDialogue(
    TargetCharacter targetCharacter)
    {
        return targetCharacter switch
        {
            TargetCharacter.Vtuber => vtuberMoneyRequestDialogue,
            TargetCharacter.OL => olMoneyRequestDialogue,
            TargetCharacter.CEO => ceoMoneyRequestDialogue,
            _ => null
        };
    }

    public RequestResult EvaluateRequest(
        TargetCharacter targetCharacter,
        RequestSize requestSize,
        AnswerQuality answerQuality)
    {
        
        GetCharacterVariableNames(
            targetCharacter,
            out string affectionVariable,
            out string suspicionVariable
        );

        int affection =
            DialogueVariableData.Instance.Get<int>(affectionVariable);

        int suspicion =
            DialogueVariableData.Instance.Get<int>(suspicionVariable);

        int requestDifficulty = GetRequestDifficulty(requestSize);
        int answerBonus = GetAnswerBonus(answerQuality);
        int requestAmount = GetRequestAmount(requestSize);

        int requestScore =
            affection
            - suspicion
            + answerBonus
            - requestDifficulty;

        bool success = requestScore >= 0;

        return new RequestResult(
            success,
            requestScore,
            requestAmount
        );
    }

    private void AddSuspicion(
    TargetCharacter targetCharacter,
    int amount)
    {
        GetCharacterVariableNames(
            targetCharacter,
            out _,
            out string suspicionVariable
        );

        DialogueVariableData.Instance.Add(
            suspicionVariable,
            amount
        );
    }

    private void AddAffection(
        TargetCharacter targetCharacter,
        int amount)
    {
        GetCharacterVariableNames(
            targetCharacter,
            out string affectionVariable,
            out _
        );

        DialogueVariableData.Instance.Add(
            affectionVariable,
            amount
        );
    }

    private void GivePlayerMoney(int amount)
    {
        DialogueVariableData.Instance.Add(
            moneyVariableName,
            amount
        );
    }

    public RequestResult MakeRequest(
        TargetCharacter targetCharacter,
        RequestSize requestSize,
        AnswerQuality answerQuality)
    {
        RequestResult result = EvaluateRequest(
            targetCharacter,
            requestSize,
            answerQuality
        );

        if (result.Success)
        {
            GivePlayerMoney(result.Amount);

            int suspicionIncrease =
                GetSuccessSuspicionIncrease(requestSize)
                + GetAnswerSuspicionModifier(answerQuality);

            suspicionIncrease = Mathf.Max(1, suspicionIncrease);

            AddSuspicion(
                targetCharacter,
                suspicionIncrease
            );
        }
        else
        {
            int suspicionIncrease =
                GetFailureSuspicionIncrease(requestSize)
                + GetAnswerSuspicionModifier(answerQuality);

            suspicionIncrease = Mathf.Max(1, suspicionIncrease);

            int affectionLoss =
                GetFailureAffectionLoss(requestSize);

            AddSuspicion(
                targetCharacter,
                suspicionIncrease
            );

            AddAffection(
                targetCharacter,
                -affectionLoss
            );
        }

        return result;
    }

    public void BeginRequest(RequestSize requestSize)
    {
        pendingTarget = dateManager.CurrentCharacter;
        pendingRequestSize = requestSize;
        hasPendingRequest = true;

        DialogueVariableData.Instance.Set(
            "CurrentRequestSize",
            requestSize.ToString()
        );

        Debug.Log(
            $"Money request started: {pendingTarget} | {pendingRequestSize}"
        );

        DialogueContainer requestDialogue =
            GetMoneyRequestDialogue(pendingTarget);

        if (requestDialogue == null)
        {
            Debug.LogWarning(
                $"No money request dialogue assigned for {pendingTarget}."
            );

            return;
        }

        dialogueLauncher.PlayDialogue(requestDialogue);

        DialogueVariableData.Instance.Set(
            "CurrentRequestSize",
            requestSize.ToString()
        );

        Debug.Log(
            "CurrentRequestSize = " +
            DialogueVariableData.Instance.Get<string>("CurrentRequestSize")
        );
    }

    
    public void BeginLittleRequest()
    {
        BeginRequest(RequestSize.Little);
    }

    public void BeginNormalRequest()
    {
        BeginRequest(RequestSize.Normal);
    }

    public void BeginBigRequest()
    {
        BeginRequest(RequestSize.Big);
    }

    public void BeginVeryBigRequest()
    {
        BeginRequest(RequestSize.VeryBig);
    }

    public RequestResult ResolveRequest(AnswerQuality answerQuality)
    {
        if (!hasPendingRequest)
        {
            Debug.LogWarning(
                "Cannot resolve money request because there is no pending request."
            );

            return default;
        }

        RequestResult result = MakeRequest(
            pendingTarget,
            pendingRequestSize,
            answerQuality
        );

        DialogueVariableData.Instance.Set(
            "LastRequestSuccess",
            result.Success
        );

        hasPendingRequest = false;

        Debug.Log(
            $"Money request resolved: " +
            $"{pendingTarget} | " +
            $"{pendingRequestSize} | " +
            $"{answerQuality} | " +
            $"Success: {result.Success} | " +
            $"Score: {result.Score} | " +
            $"Amount: {result.Amount}"
        );

        return result;
    }

    public void ResolveVeryConvincingAnswer()
    {
        ResolveRequest(AnswerQuality.VeryConvincing);
    }

    public void ResolveGoodAnswer()
    {
        ResolveRequest(AnswerQuality.Good);
    }

    public void ResolveNeutralAnswer()
    {
        ResolveRequest(AnswerQuality.Neutral);
    }

    public void ResolveSuspiciousAnswer()
    {
        ResolveRequest(AnswerQuality.Suspicious);
    }

    public void ResolveExtremelySuspiciousAnswer()
    {
        ResolveRequest(AnswerQuality.ExtremelySuspicious);
    }

}