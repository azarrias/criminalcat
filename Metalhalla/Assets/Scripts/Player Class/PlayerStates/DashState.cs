using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{
    int dashFramesDuration;
    int dashFramesCount;

    public DashState(int framesDuration)
    {
        dashFramesDuration = framesDuration;
        dashFramesCount = 0;
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
            dashFramesCount = 0;

        if (status.jumpAvailable == true && (input.newInput.GetJumpButtonDown() == true || input.newInput.GetJumpButtonHeld() == true))
        {
            status.SetState(PlayerStatus.jump);
            return;
        }

        if (dashFramesCount >= dashFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);
            return;
        }
        else
        {
            status.SetState(this);
        }

        dashFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }
}
