using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillState : PlayerState
{
    int refillFramesDuration;
    int refillFramesCount;

    public RefillState( int framesDuration )
    {
        refillFramesDuration = framesDuration;
        refillFramesCount = 0; 
    }
    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            refillFramesCount = 0;
            if (status.RefillBeer(5) == false)
            {
                status.SetState(PlayerStatus.idle);
                return;
            }
        }

        

        if (refillFramesCount >= refillFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);

        }
        else
            status.SetState(this);

        refillFramesCount++;

    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {

    }

}
