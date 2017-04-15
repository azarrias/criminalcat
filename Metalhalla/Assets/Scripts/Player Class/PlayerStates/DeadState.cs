using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{
    int deadFramesDuration;
    int deadFramesCount;

    public DeadState( int animationFramesDuration)
    {
        deadFramesDuration = animationFramesDuration;
        deadFramesCount = 0; 
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            deadFramesCount = 0;
            status.StartRespawnCameraFade();
        }

        if (deadFramesCount >= deadFramesDuration)
        {
            status.SetState(PlayerStatus.idle);
            status.SetMaxHealth();
            status.SetPlayerAtRespawnPoint();
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
