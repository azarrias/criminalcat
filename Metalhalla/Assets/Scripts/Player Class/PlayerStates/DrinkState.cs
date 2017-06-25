using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkState : PlayerState
{
    int drinkFramesDuration;
    int drinkFramesCount;

    public DrinkState(int framesDuration)
    {
        drinkFramesDuration = framesDuration;
        drinkFramesCount = 0;
    }
    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            drinkFramesCount = 0;
            if (status.ConsumeBeer(1) == false) 
            {
                status.SetState(PlayerStatus.idle);
                return;
            }
            status.StartGUIFeedback("YButton");
        }

        if (drinkFramesCount >= drinkFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);

        }
        else
            status.SetState(this);

        drinkFramesCount++;

    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
