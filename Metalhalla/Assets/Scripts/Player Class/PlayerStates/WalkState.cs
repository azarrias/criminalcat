using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerState
{

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (input.newInput.GetVerticalInput() != 0 && status.climbLadderAvailable == true)
        {
            status.SetState(PlayerStatus.climb);
            return;
        }

        if (input.newInput.GetAttackButtonDown() == true )
        {
            status.SetState(PlayerStatus.attack);
            return; 
        }

        if (input.newInput.GetJumpButtonDown() == true)
        {
            status.SetState(PlayerStatus.jump);
            return;
        }

        if ((input.newInput.GetHorizontalInput() < 0 && status.facingRight) || (input.newInput.GetHorizontalInput() > 0 && !status.facingRight))
            status.Flip();

    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {
        if (collider.collisions.below == false)
            status.SetState(PlayerStatus.fall);
    }

}
