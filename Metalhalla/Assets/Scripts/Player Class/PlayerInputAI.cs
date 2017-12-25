using UnityEngine;
using System.Collections;

public class PlayerInputAI : PlayerInput
{
    [System.Serializable]
    public enum AIBehaviourOnEnd
    {
        None,
        Loop, 
        ReturnControlToPlayer
    }

    public enum AIProgram
    {
        None,
        AfterBossDefeat,
        MainMenuAnimation,
        VictoryPose,
        WalkRight
    };

    [System.Serializable]
    public enum AIAction
    {
        None,
        Idle,
        WalkRight,
        WalkLeft,
        Jump,
        Attack,
        Dash,
        Taunt,
        ShieldUp,
        HammerCheck
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
    private AIStep[] stepsAfterBossDefeat = {   new AIStep(AIAction.Idle, 0.2f),
                                                new AIStep(AIAction.WalkRight, 1.5f),
                                                new AIStep(AIAction.ShieldUp, 10.0f)
                                               };
    public AIBehaviourOnEnd behaviourAfterBossDefeat = AIBehaviourOnEnd.None;

    [Header("MainMenuAnimation program parameters")]
    [SerializeField]
    private AIStep[] stepsMainMenuAnimation = { new AIStep(AIAction.Idle, 0.5f ),
                                                new AIStep(AIAction.Attack,1.0f),
                                                new AIStep(AIAction.Jump, 1.0f) };
    public AIBehaviourOnEnd behaviourMainMenuAnimation = AIBehaviourOnEnd.Loop;

    [Header("MainMenuAnimation program parameters")]
    [SerializeField]
    private AIStep[] stepsWalkRightAI= { new AIStep(AIAction.WalkRight, 5.0f )};
    public AIBehaviourOnEnd behaviourWalkRightAI = AIBehaviourOnEnd.Loop;

    [Header("VictoryPose program parameters")]
    [SerializeField]
    private AIStep[] stepsVictoryPose = { new AIStep(AIAction.WalkLeft, 0.7f),
                                          new AIStep(AIAction.Idle, 0.1f),
                                          new AIStep(AIAction.Taunt, 1.3f),
                                          new AIStep(AIAction.WalkRight, 0.7f),
                                          new AIStep(AIAction.Idle, 0.1f),
                                          new AIStep(AIAction.Taunt, 1.3f)};
    private AIBehaviourOnEnd behaviourVictoryPose = AIBehaviourOnEnd.ReturnControlToPlayer;

    private AIStep[] _currentProgramSteps;
    private int _currentProgramStepIndex;
    private AIBehaviourOnEnd _currentProgramBehaviour;
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


    private void SetAIProgramConstraints(AIStep[] steps, AIBehaviourOnEnd behaviour)
    {
        _currentProgramSteps = steps;
        _currentProgramBehaviour = behaviour;
        _currentProgramStepIndex = 0;
        _currentProgramTime = -0.001f;
    }
    private void ExecuteCurrentAIProgram()
    {
        newInput.Reset();

        AIAction stepAction = _currentProgramSteps[_currentProgramStepIndex].action;
        float stepTime = _currentProgramSteps[_currentProgramStepIndex].duration;
        //Debug.Log(stepAction + " => " + stepTime);

        switch (stepAction)
        {
            case AIAction.WalkRight:
                newInput.SetHorizontalInput(1.0f);
                break;
            case AIAction.WalkLeft:
                newInput.SetHorizontalInput(-1.0f);
                break;
            case AIAction.Jump:
                if (_currentProgramTime <= 0.2f)
                    newInput.SetJumpButtonDown(true);
                break;
            case AIAction.Attack:
                if (_currentProgramTime <= 0.2f)
                    newInput.SetAttackButtonDown(true);
                break;
            case AIAction.Dash:
                if (_currentProgramTime <= 0.2f)
                    newInput.SetDashButtonDown(true);
                break;
            case AIAction.Taunt:
                if (_currentProgramTime <= 0.2f)
                    newInput.SetTauntButtonDown(true);
                break;
            case AIAction.ShieldUp:
                {
                    if (_currentProgramTime <= 0.2f)
                        newInput.SetDefenseButtonDown(true);
                    else
                        newInput.SetDefenseButtonHeld(true);
                    // move from forward to up
                    float lambda = (0.3f - _currentProgramTime)/0.3f;
                    lambda = Mathf.Clamp(lambda, 0, 1);
                    newInput.SetHorizontalInput(lambda);
                    newInput.SetVerticalInput(1 - lambda);
                    break;
                }
            case AIAction.HammerCheck:
                GetComponent<Animator>().SetLayerWeight(1, 1);
                break;
            case AIAction.None:
            default:
                break;
        }
        
        // PlayerStatus blends automatically the layerWeight when showing the shield up
        if (stepAction  != AIAction.ShieldUp && stepAction != AIAction.HammerCheck)
            GetComponent<Animator>().SetLayerWeight(1, 0);

        _currentProgramTime += Time.deltaTime;
        if (_currentProgramTime >= stepTime)
        {
            int totalSteps = _currentProgramSteps.Length;
            _currentProgramStepIndex++;

            switch (_currentProgramBehaviour)
            {
                case AIBehaviourOnEnd.Loop:
                    _currentProgramStepIndex = (_currentProgramStepIndex) % totalSteps;
                    _currentProgramTime = 0.0f;
                    break;
                case AIBehaviourOnEnd.None:
                    if (_currentProgramStepIndex != totalSteps)
                    {
                        _currentProgramTime = 0.0f;
                    }
                    break;
                case AIBehaviourOnEnd.ReturnControlToPlayer:
                    if (_currentProgramStepIndex != totalSteps)
                    {
                        _currentProgramTime = 0.0f;
                    }
                    else
                    {
                        GetComponent<PlayerController>().switchAIInput(false);
                    }
                    break;
            }
        }

    }

    public void SetAIProgram(AIProgram program)
    {
        switch (program)
        {
            case AIProgram.AfterBossDefeat:
                SetAIProgramConstraints(stepsAfterBossDefeat, behaviourAfterBossDefeat);
                break;
            case AIProgram.MainMenuAnimation:
                SetAIProgramConstraints(stepsMainMenuAnimation, behaviourMainMenuAnimation);
                break;
            case AIProgram.WalkRight:
                SetAIProgramConstraints(stepsWalkRightAI, behaviourWalkRightAI);
                break;

            case AIProgram.None:
            case AIProgram.VictoryPose:
                SetAIProgramConstraints(stepsVictoryPose, behaviourVictoryPose);
                break;
        }
    }
}
