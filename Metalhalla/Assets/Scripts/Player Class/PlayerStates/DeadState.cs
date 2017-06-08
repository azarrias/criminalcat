using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{
    int deadFramesDuration;
    int deadFramesCount;

    int frameToReduceCollider;
    int frameToRestoreCollider;

    public DeadState( int animationFramesDuration)
    {
        deadFramesDuration = animationFramesDuration;
        deadFramesCount = 0;
        frameToReduceCollider = 15;
        frameToRestoreCollider = deadFramesDuration -1;
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
            if (status.facingRight == false)
                status.Flip();
            status.SetState(PlayerStatus.idle);
            status.SetMaxHealth();
            status.SetPlayerAtRespawnPoint();
            // add hoc for level elements
            GameObject.FindGameObjectWithTag("MovingDoor").GetComponent<CloseOpenDoor>().OpenDoor();

            return;
        }
        else
        {
            status.SetState(this);
            if (deadFramesCount == frameToReduceCollider)
                status.SetColliderYSize(1.1f);
            if (deadFramesCount == frameToRestoreCollider)
                status.RestoreColliderSize();
        }
        deadFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
