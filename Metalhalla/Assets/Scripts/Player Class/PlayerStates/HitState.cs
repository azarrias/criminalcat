using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : PlayerState
{
    int hitFramesDuration;
    int hitFramesCount;

    public HitState( int framesDuration )
    {
        hitFramesCount = 0;
        hitFramesDuration = framesDuration;
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
            hitFramesCount = 0;

        if (hitFramesCount >= hitFramesDuration)
            status.SetState(PlayerStatus.idle);
        else
            status.SetState(this);

        hitFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {

    }

}
