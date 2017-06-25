using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : PlayerState
{
    int hitFramesDuration;
    int hitFramesCount;
    int justHitFramesActive = 4;

    public HitState( int framesDuration )
    {
        hitFramesCount = 0;
        hitFramesDuration = framesDuration;
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            hitFramesCount = 0;
            status.justHit = true;
            if (status.IsAlive())
                status.PlayFx("hurtScream");
        }
        
        if (hitFramesCount >= hitFramesDuration)
        {
            if (status.IsAlive())
                status.SetState(PlayerStatus.idle);
            else
                status.SetState(PlayerStatus.dead);
            return;
        }
        else
        {
            status.SetState(this);
            if (hitFramesCount > justHitFramesActive)
                status.justHit = false;
        }

        hitFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
