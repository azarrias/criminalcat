using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbState : PlayerState
{

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
            status.SetClimbStateModelRotation(); 

        status.playerAnimator.speed = 1;

        if (input.newInput.GetJumpButtonDown() == true || status.climbLadderAvailable == false)
        {
            status.SetState(PlayerStatus.fall);
            status.SetInitialModelRotation();
        }
        else
        {
            status.SetState(PlayerStatus.climb);
            if (input.newInput.GetVerticalInput() == 0 && input.newInput.GetHorizontalInput() == 0)
                status.playerAnimator.speed = 0;
        }
        
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {
        if (collider.collisions.below == true)
        {
            status.SetState(PlayerStatus.idle);
            status.SetInitialModelRotation();
        }
    }

}
