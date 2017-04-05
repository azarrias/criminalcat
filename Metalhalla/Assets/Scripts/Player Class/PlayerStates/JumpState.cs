using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    int framesToJumpCount = 0;
    int framesToJumpMax;
    int framesToJumpMin;


    public JumpState( int framesToMaxJump)
    {
        framesToJumpCount = 0;
        framesToJumpMax = framesToMaxJump;
        framesToJumpMin = (int)(framesToJumpMax / 4);

    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
            framesToJumpCount = 0;

        if (status.climbLadderAvailable == true && input.newInput.GetVerticalInput() != 0)
            status.SetState(PlayerStatus.climb);
        else
        {
            if ((input.newInput.GetHorizontalInput() < 0 && status.facingRight) || (input.newInput.GetHorizontalInput() > 0 && !status.facingRight))
                status.Flip();


            if (input.newInput.GetJumpButtonHeld() && (framesToJumpCount <= framesToJumpMax))
            {
                status.SetState(this);
                framesToJumpCount++;
            }
            else
            {
                if (framesToJumpCount > framesToJumpMin)
                {
                    status.SetState(PlayerStatus.fall);
                    framesToJumpCount = 0;
                }
                else
                {
                    status.SetState(this);
                    framesToJumpCount++;
                }
            }
        }
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {
        if (collider.collisions.above == true)
            status.SetState(PlayerStatus.fall);
    }
}
