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
        //framesToJumpMin = (int)(framesToJumpMax / 4);
        framesToJumpMin = framesToJumpMax;

    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            framesToJumpCount = 0;
            status.PlayFx("jump");
        }

        // counter for PlayerMove calculations
        if (framesToJumpCount < framesToJumpMax)
            status.jumpFrames = framesToJumpCount;

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

        if (input.newInput.GetDashButtonDown() == true)
        {
            status.SetState(PlayerStatus.dash);
            return;
        }

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

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {
        if (collider.collisions.above == true)
            status.SetState(PlayerStatus.fall);
    }
}
