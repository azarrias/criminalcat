using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerState
{

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if ((input.newInput.GetHorizontalInput() < 0 && status.facingRight) || (input.newInput.GetHorizontalInput() > 0 && !status.facingRight))
            status.Flip();
        status.SetState(this); 
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {
        if (collider.collisions.below)
            status.SetState(PlayerStatus.idle); 
    }

}
