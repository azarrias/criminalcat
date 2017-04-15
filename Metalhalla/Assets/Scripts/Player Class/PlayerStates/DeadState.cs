using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{
    int deadFramesDuration;
    int deadFramesCount;

    public DeadState( int framesDuration)
    {
        deadFramesDuration = framesDuration;
        deadFramesCount = 0; 
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
            deadFramesCount = 0;

        if (deadFramesCount >= deadFramesDuration)
        {
            status.SetState(PlayerStatus.idle);
            status.SetMaxHealth();
            return;
        }
        else
            status.SetState(this);
        deadFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
