using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseState : PlayerState
{

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (input.newInput.GetDefenseButtonDown() == true || input.newInput.GetDefenseButtonHeld() == true )
        {
            status.SetState(this);
            return;
        }

        if (input.newInput.GetHorizontalInput() != 0)
            status.SetState(PlayerStatus.walk);
        else
            status.SetState(PlayerStatus.idle);

    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
