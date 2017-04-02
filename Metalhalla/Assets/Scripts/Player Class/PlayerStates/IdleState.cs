﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState {

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        Debug.Log("Idle.HandleInput()");

        if (input.newInput.GetVerticalInput() != 0 && status.climbLadderAvailable == true)
        {
            status.SetState(PlayerStatus.climb); 
            return; 
        }

        if ((input.newInput.GetHorizontalInput() < 0 && status.facingRight) || (input.newInput.GetHorizontalInput() > 0 && !status.facingRight))
        {
            status.SetState(PlayerStatus.walk); 
            status.Flip();
        }

        if (input.newInput.GetJumpButtonDown() == true)
        {
            status.SetState(PlayerStatus.jump); 
        }
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {
        if (collider.collisions.below == false)
            status.SetState(PlayerStatus.fall);
    }

}
