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

            if (status.magic == PlayerStatus.MAGIC.EAGLE)
            {
                status.PlayFx("tornado");
                // New version with ParticlesManager
                if (status.facingRight)
                    ParticlesManager.SpawnParticle("tornado", status.transform.position + status.eagleAttackInstanceOffset, true);
                else
                    ParticlesManager.SpawnParticle("tornado", status.transform.position - status.eagleAttackInstanceOffset.x * Vector3.right + status.eagleAttackInstanceOffset.y * Vector3.up, false);
            }
            
            if (status.magic == PlayerStatus.MAGIC.WILDBOAR)
            {
                if (status.facingRight)
                    ParticlesManager.SpawnParticle("wildboar", status.transform.position + status.wildboarAttackInstanceOffset, true);
                else
                    ParticlesManager.SpawnParticle("wildboar", status.transform.position - status.wildboarAttackInstanceOffset.x * Vector3.right + status.wildboarAttackInstanceOffset.y * Vector3.up, false);
            }
            /*
            else
            {
                status.SetState(PlayerStatus.idle);
                return;
            }*/

        }

        if (castFramesCount>= castFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);
        }
        else
            status.SetState(this);

        castFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
