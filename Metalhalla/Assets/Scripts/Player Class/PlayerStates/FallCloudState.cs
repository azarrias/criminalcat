using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCloudState : PlayerState {

    int fallCloudFramesCount;
    int fallCloudFramesDuration;

    public FallCloudState( int framesDuration)
    {
        fallCloudFramesDuration = framesDuration;
        fallCloudFramesCount = 0;
    }
    
    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        Debug.Log("FallCloudState active");
        if (status.previousState != this)
            fallCloudFramesCount = 0;

        if (fallCloudFramesCount >= fallCloudFramesDuration)
            status.SetState(PlayerStatus.fall);
        else
            status.SetState(this);

        fallCloudFramesCount++;

    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {

    }

}