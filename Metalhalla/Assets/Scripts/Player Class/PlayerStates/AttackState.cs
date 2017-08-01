using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    int attackFramesDuration;
    int attackFramesCount;
    int attackColliderDelay;

    public AttackState( int framesDuration ) 
    {
        attackFramesDuration = framesDuration;
        attackFramesCount = 0;
        attackColliderDelay = framesDuration / 2;   // TODO: adjust times to frame precision
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            attackFramesCount = 0;
            status.lightningGenerator.SetActive(true);  // lightning on
        }

        if ( attackColliderDelay <= attackFramesCount && status.attackCollider.enabled == false)
        {
            status.attackCollider.enabled = true;
            status.PlayFx("swing");
        }
        
        status.SetState(this);
        attackFramesCount++; 
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {
        if ( collider.IsGrounded() && attackFramesCount >= attackFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);
            status.attackCollider.enabled = false;
            status.lightningGenerator.SetActive(false);  // lightning off
            attackFramesCount = 0; 
        }
    }

}
