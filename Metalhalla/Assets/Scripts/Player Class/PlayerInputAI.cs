using UnityEngine;
using System.Collections;

public class PlayerInputAI : PlayerInput
{

    public enum AIProgram
    {
        None,
        AfterBossDefeat,
        MainMenuAnimation
    };[System.Serializable]
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
    private AIStep[] stepsMainMenuAnimation = { new AIStep( AIAction.Idle, 0.5f ),
                                                new AIStep(AIAction.Walk, 2.0f),
                                                new AIStep(AIAction.Dash) };
    public bool loopMainMenuAnimation = true;

    public PlayerInputAI() : base()
    {
    }

    private void Start()
    {
        base.StartPlayerInput();    // substitute to base.Start() due to protection level and override not feasible
    }

    public override void GetInput()
    {
        switch (program)
        {
            case AIProgram.None: break;
            case AIProgram.AfterBossDefeat:
                UpdateRunRightToDie();
                break;
            case AIProgram.MainMenuAnimation:
                UpdateMainMenuAnimation();
                break;
        }
    }
    private void UpdateRunRightToDie()
    {

    }
    private void UpdateMainMenuAnimation()
    {

    }
}
