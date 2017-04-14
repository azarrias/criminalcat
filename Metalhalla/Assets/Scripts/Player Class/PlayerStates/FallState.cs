using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerState
{

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.climbLadderAvailable == true && input.newInput.GetVerticalInput() > 0)
        {
            status.SetState(PlayerStatus.climb);
            return;
        }

        if (input.newInput.GetAttackButtonDown() == true)
        {
            status.SetState(PlayerStatus.attack);
            return;
        }

        if ((input.newInput.GetHorizontalInput() < 0 && status.facingRight) || (input.newInput.GetHorizontalInput() > 0 && !status.facingRight))
            status.Flip();

        status.SetState(this); 
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {
        if (collider.collisions.below)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);
        }
    }

}
