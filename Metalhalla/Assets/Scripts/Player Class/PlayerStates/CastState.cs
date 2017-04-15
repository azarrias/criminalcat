using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastState : PlayerState
{
    int castFramesDuration;
    int castFramesCount;

    public CastState(int framesDuration)
    {
        castFramesCount = 0;
        castFramesDuration = framesDuration;
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            castFramesCount = 0;
            if (status.ConsumeStamina(7) == false)  // TODO: put stamina consumption depending on the cast
            {
                status.SetState(PlayerStatus.idle);
                return;
            }
        }

        // add instantiation of cast attack based on the input - Eagle or Wild Boar attack

        if (castFramesCount>= castFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);
            status.attackCollider.enabled = false;

        }
        else
            status.SetState(this);

        castFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
