using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastState : PlayerState
{
    int castFramesDuration;
    int castFramesCount;

    public CastState(int framesDuration)
    {
        castFramesCount = 0;
        castFramesDuration = framesDuration;
    }

    public override void HandleInput(PlayerInput input, PlayerStatus status)
    {
        if (status.previousState != this)
        {
            castFramesCount = 0;
            // instantiation of ONLY Eagle, need to modify when having wild boar attack
            if (input.newInput.GetHorizontalInput() != 0 && status.ConsumeStamina(7) == true )
            {
                GameObject cast = Instantiate(status.eagleAttack);
                cast.GetComponent<TornadoBehaviour>().SetFacingRight(status.facingRight);
                if (status.facingRight == true)
                    cast.transform.position = status.transform.position + status.eagleAttackInstanceOffset;
                else
                    cast.transform.position = status.transform.position - status.eagleAttackInstanceOffset.x * Vector3.right + status.eagleAttackInstanceOffset.y * Vector3.up;
            }
            else
            {
                status.SetState(PlayerStatus.idle);
                return;
            }
        }

        if (castFramesCount>= castFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);
            status.attackCollider.enabled = false;

        }
        else
            status.SetState(this);

        castFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
