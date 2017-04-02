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

    public void statusUpdateAfterInput( PlayerInput input)
    {
        currentState.HandleInput(input, this); 
    }

    public void statusUpdateAfterCollisionCheck( PlayerCollider collider )
    {
        currentState.UpdateAfterCollisionCheck(collider, this); 
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

    /*   
       public void statusUpdateAfterInput(PlayerInput input) {

               if (oldStatus.IsFallThroughCloudPlatform())
               {
                   if (framesToFallThroughCloudPlatformsCount <= framesToFallThroughCloudPlatforms)
                   {
                       newStatus.SetFallThroughCloudPlatform();
                       framesToFallThroughCloudPlatformsCount++;
                   }
                   else
                   {
                       newStatus.SetFall();
                       framesToFallThroughCloudPlatformsCount = 0;
                   }
                   return;
               }
           }
       }

       public void statusUpdateAfterCollisionCheck( PlayerCollider collider)
       {
           if (newStatus.IsFallThroughCloudPlatform()) // no vertical collisions here
           {
               newStatus.IsFallThroughCloudPlatform();
               return;
           }

       }

*/

}
