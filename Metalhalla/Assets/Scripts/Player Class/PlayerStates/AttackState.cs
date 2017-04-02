using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    int attackFramesDuration;
    int attackFramesCount; 

    public AttackState( int framesDuration )
    {
        attackFramesDuration = framesDuration;
        attackFramesCount = 0; 
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
            attackFramesCount = 0;

        if (attackFramesCount >= attackFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);

        }
        else
            status.SetState(this);

        attackFramesCount++; 
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {

    }

}
