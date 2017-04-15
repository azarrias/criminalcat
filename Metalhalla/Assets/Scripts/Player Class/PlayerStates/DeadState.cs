using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        Debug.Log("Dead");
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
