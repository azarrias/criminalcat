﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerState
{

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState == PlayerStatus.fall && input.newInput.GetJumpButtonHeld() == true)
            status.jumpAvailable = false;
        else if (status.jumpAvailable == false && input.newInput.GetJumpButtonHeld() == false)
            status.jumpAvailable = true;

        if (input.newInput.GetVerticalInput() != 0 && status.climbLadderAvailable == true)
        {
            status.SetState(PlayerStatus.climb);
            return;
        }

        if (input.newInput.GetDashButtonDown() == true || input.newInput.GetHorizontalDoubleTap() == true)
        {
            status.SetState(PlayerStatus.dash);
            return;
        }

        if (input.newInput.GetAttackButtonDown() == true)
        {
            status.SetState(PlayerStatus.attack);
            return;
        }

        if (input.newInput.GetCastButtonDown() == true && status.ConsumeStamina(1) == true)
        {
            status.SetState(PlayerStatus.cast);
            return;
        }

        if (input.newInput.GetDefenseButtonDown() == true || input.newInput.GetDefenseButtonHeld() == true)
        {
            status.SetState(PlayerStatus.defense);
            return;
        }

        if (status.jumpAvailable == true && (input.newInput.GetJumpButtonDown() == true || input.newInput.GetJumpButtonHeld() == true))
        {
            //if (input.newInput.GetVerticalInput() < 0 && status.GetComponent<PlayerCollider>().PlayerAboveCloudPlatform() == true)
            //{
            //    status.SetState(PlayerStatus.fallcloud);
            //    return;
            //}
            //else
            //{
                status.SetState(PlayerStatus.jump);
                return;
            //}
        }

        if (input.newInput.GetContextButtonDown() == true && status.ConsumeBeer(1) == true)
        {
            status.SetState(PlayerStatus.drink);
            return;
        }


        if (input.newInput.GetHorizontalInput() == 0)
        { 
            status.SetState(PlayerStatus.idle);
            return;
        }

        if ((input.newInput.GetHorizontalInput() < 0 && status.facingRight) || (input.newInput.GetHorizontalInput() > 0 && !status.facingRight))
            status.Flip();
        status.SetAnimatorWalkingSpeed(input.newInput.GetHorizontalInput());
        status.SetState(this);
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {
        if (collider.IsGrounded() == false)
        {
            status.SetState(PlayerStatus.fall);
            status.framesInDelayCount = 0;
        }

        if ((collider.collisions.left == true && input.newInput.GetHorizontalInput() < 0) || (collider.collisions.right == true && input.newInput.GetHorizontalInput() > 0))
            status.SetState(PlayerStatus.idle);

    }

}
