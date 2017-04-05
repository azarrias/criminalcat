﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState {

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState == PlayerStatus.fall && input.newInput.GetJumpButtonHeld() == true)
            status.jumpAvailable = false;
        else if( status.jumpAvailable == false && input.newInput.GetJumpButtonHeld() == false)
            status.jumpAvailable = true;

        if (input.newInput.GetVerticalInput() != 0 && status.climbLadderAvailable == true)
        {
            status.SetState(PlayerStatus.climb); 
            return; 
        }

        if (input.newInput.GetAttackButtonDown() == true)
        {
            status.SetState(PlayerStatus.attack);
            return; 
        }

        if (input.newInput.GetDefenseButtonDown() == true || input.newInput.GetDefenseButtonHeld() == true)
        {
            status.SetState(PlayerStatus.defense);
            return;
        }


        if (status.jumpAvailable == true && (input.newInput.GetJumpButtonDown() == true || input.newInput.GetJumpButtonHeld() == true))
        {
            status.SetState(PlayerStatus.jump);
            return;
        }

        //if ((input.newInput.GetHorizontalInput() < 0 && status.facingRight) || (input.newInput.GetHorizontalInput() > 0 && !status.facingRight))
        if (input.newInput.GetHorizontalInput() != 0)
        {
            status.SetState(PlayerStatus.walk); 
            return;
        }

        status.SetState(this);
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status)
    {
        if (collider.IsGrounded() == false)
            status.SetState(PlayerStatus.fall);

    }

}
