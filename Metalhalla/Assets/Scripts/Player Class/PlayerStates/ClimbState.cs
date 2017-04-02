using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbState : PlayerState
{

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        Debug.Log("Climb.HandleInput()");
        if (input.newInput.GetHorizontalInput() != 0 || status.climbLadderAvailable == false )
        {
            status.SetState(PlayerStatus.fall);
        }
        else
            status.SetState(PlayerStatus.climb); 
        
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {
        if (collider.collisions.below == true)
            status.SetState(PlayerStatus.idle);
    }

}
