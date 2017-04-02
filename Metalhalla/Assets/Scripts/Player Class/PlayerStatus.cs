using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {

    // -- State pattern --
    public static IdleState idle; 
    public static JumpState jump; 
    public static FallState fall;
    public static WalkState walk;
    public static ClimbState climb; 
    /*
    public static AttackState attack;
    public static FallState fall;
    public static HitState hit; 
    public static DeadState dead; 
    */

    [HideInInspector]
    public PlayerState currentState;
    [HideInInspector]
    public PlayerState previousState; 

    int framesToFallThroughCloudPlatforms;
    int framesToFallThroughCloudPlatformsCount;

    [HideInInspector]
    public bool facingRight;
    [HideInInspector]
    public bool climbLadderAvailable;
    [HideInInspector]
    public bool beerRefillAvailable;

    void Start()
    {
        idle = new IdleState();
        jump = new JumpState(CalculateFramesFromTime(GetComponent<PlayerMove>().timeToJumpApex));
        fall = new FallState();
        walk = new WalkState();
        climb = new ClimbState(); 

        SetState(idle); 

        framesToFallThroughCloudPlatformsCount = 0;
        framesToFallThroughCloudPlatforms = CalculateFramesFromTime(GetComponent<PlayerMove>().timeToFallThroughCloudPlatforms);

        facingRight = true;
        climbLadderAvailable = false;
        beerRefillAvailable = false;
  
    }

    //--- handled by each of the states
    public void statusUpdateAfterInput( PlayerInput input)
    {
        currentState.HandleInput(input, this); 
    }

    public void statusUpdateAfterCollisionCheck( PlayerCollider collider )
    {
        currentState.UpdateAfterCollisionCheck(collider, this); 
    }

    // get animator parameters
    public bool IsGrounded()
    {
        if (currentState == idle || currentState == walk)
            return true;
        else
            return false;
    }




    int CalculateFramesFromTime(float time) { return (int)(time / Time.fixedDeltaTime); }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 tmp = transform.localScale;
        tmp.x *= -1;
        transform.localScale = tmp;

    }

    public void SetState( PlayerState newState)
    {
        previousState = currentState;
        currentState = newState; 
    }
}
