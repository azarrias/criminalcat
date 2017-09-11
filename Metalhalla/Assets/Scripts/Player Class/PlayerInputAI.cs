using UnityEngine;
using System.Collections;

public class PlayerInputAI : PlayerInput
{

    public enum AIProgram
    {
        None,
        AfterBossDefeat,
        MainMenuAnimation,
        WalkRight
    };

    [System.Serializable]
    public enum AIAction
    {
        None,
        Idle,
        Walk,
        Jump,
        Attack,
        Dash
    };

    [System.Serializable]
    public class AIStep
    {
        public AIAction action;
        public float duration;

        public AIStep(AIAction a, float d = 0.0f)
        {
            action = a;
            duration = d;
        }
    };
    public AIProgram program;

    [Header("AfterBossDefeat program parameters")]
    [SerializeField]
    private AIStep[] stepsAfterBossDefeat = { new AIStep( AIAction.Idle, 0.5f ),
                                                new AIStep(AIAction.Walk, 2.0f),
                                                new AIStep(AIAction.Dash) };
    public bool loopAfterBossDefeat = false;

    [Header("MainMenuAnimation program parameters")]
    [SerializeField]
    private AIStep[] stepsMainMenuAnimation = { new AIStep(AIAction.Idle, 0.5f ),
                                                new AIStep(AIAction.Attack,1.0f),
                                                new AIStep(AIAction.Jump, 1.0f) };
    public bool loopMainMenuAnimation = true;

    [Header("MainMenuAnimation program parameters")]
    [SerializeField]
    private AIStep[] stepsWalkRightAI= { new AIStep(AIAction.Walk, 5.0f )};
    public bool loopWalkRightAI= true;


    private AIStep[] _currentProgramSteps;
    private int _currentProgramStepIndex;
    private bool _currentProgramLoop;
    private float _currentProgramTime;
    
    public PlayerInputAI() : base()
    {
    }

    private void Start()
    {
        base.StartPlayerInput();    // substitute to base.Start() due to protection level and override not feasible
        SetAIProgram( program );
    }

    public override void GetInput()
    {
        ExecuteCurrentAIProgram();
    }


    private void SetAIProgramConstraints(AIStep[] steps, bool loop)
    {
        _currentProgramSteps = steps;
        _currentProgramLoop = loop;
        _currentProgramStepIndex = 0;
        _currentProgramTime = -0.001f;
    }
    private void ExecuteCurrentAIProgram()
    {
        newInput.Reset();

        AIAction stepAction = _currentProgramSteps[_currentProgramStepIndex].action;
        float stepTime = _currentProgramSteps[_currentProgramStepIndex].duration;

        if (stepAction == AIAction.Walk)
        {
            newInput.SetHorizontalInput(1.0f);
        }
        else if (_currentProgramTime <= 0.0f)
        {
            switch(stepAction)
            {
                case AIAction.Jump:
                    newInput.SetJumpButtonDown(true);
                    break;
                case AIAction.Attack:
                    newInput.SetAttackButtonDown(true);
                    break;
                case AIAction.Dash:
                    newInput.SetDashButtonDown(true);
                    break;
                default: break;
            }
        }
        _currentProgramTime += Time.deltaTime;
        if (_currentProgramTime >= stepTime)
        {
            int totalSteps = _currentProgramSteps.Length;
            if (_currentProgramLoop)
            {
                _currentProgramStepIndex = (_currentProgramStepIndex + 1) % totalSteps;
                _currentProgramTime = -0.001f;
            }
            else
            {
                if (_currentProgramStepIndex + 1 != totalSteps)
                {
                    _currentProgramStepIndex++;
                    _currentProgramTime = -0.001f;
                }
            }

        }

    }

    public void SetAIProgram(AIProgram program)
    {
        switch (program)
        {
            case AIProgram.None: break;
            case AIProgram.AfterBossDefeat:
                SetAIProgramConstraints(stepsAfterBossDefeat, loopAfterBossDefeat);
                break;
            case AIProgram.MainMenuAnimation:
                SetAIProgramConstraints(stepsMainMenuAnimation, loopMainMenuAnimation);
                break;
            case AIProgram.WalkRight:
                SetAIProgramConstraints(stepsWalkRightAI, loopWalkRightAI);
                break;
        }
    }
}
