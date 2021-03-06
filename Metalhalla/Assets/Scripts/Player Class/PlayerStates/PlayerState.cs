﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState 
{
     ~PlayerState() { }
    public virtual void HandleInput(PlayerInput input, PlayerStatus status) { }
    public virtual void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input) { }
}
