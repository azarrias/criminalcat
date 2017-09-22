using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntState : PlayerState
{
    int tauntFramesDuration;
    int tauntFramesCount;

    public TauntState(int framesDuration)
    {
        tauntFramesDuration = framesDuration;
        tauntFramesCount = 0;
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            tauntFramesCount = 0;
        }

        if (tauntFramesCount >= tauntFramesDuration)
        {
            status.SetState(PlayerStatus.idle);
            return;
        }
        else
            status.SetState(this);

        tauntFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }
}
