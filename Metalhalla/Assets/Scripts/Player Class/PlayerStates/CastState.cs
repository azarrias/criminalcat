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

            // eagle attack: idle, left or right pressed + cast button
            //if (input.newInput.GetHorizontalInput() != 0 && status.ConsumeStamina(1) == true )
            if (input.newInput.GetVerticalInput() == 0 )
            {
                // New version with ParticlesManager
                // ParticlesManager.SpawnParticle("tornado", status.transform.position, status.facingRight);
                GameObject cast = Instantiate(status.eagleAttack);
                cast.GetComponent<TornadoBehaviour>().SetFacingRight(status.facingRight);
                if (status.facingRight == true)
                    cast.transform.position = status.transform.position + status.eagleAttackInstanceOffset;
                else
                    cast.transform.position = status.transform.position - status.eagleAttackInstanceOffset.x * Vector3.right + status.eagleAttackInstanceOffset.y * Vector3.up;

            }
            // boar attack
            else if (input.newInput.GetVerticalInput() < 0 )
            {
                // New version with ParticlesManager
                //ParticlesManager.SpawnParticle("wildboar", status.transform.position, status.facingRight);
                Debug.Log("Wild boar attack");
            }
            else
            {
                status.SetState(PlayerStatus.idle);
                return;
            }

            status.hammerMesh.GetComponent<Renderer>().enabled = true;  // to remove when cast animation is ready, using attack animation
        }

        if (castFramesCount>= castFramesDuration)
        {
            if (input.newInput.GetHorizontalInput() != 0)
                status.SetState(PlayerStatus.walk);
            else
                status.SetState(PlayerStatus.idle);
            status.hammerMesh.GetComponent<Renderer>().enabled = false; // to remove when cast animation is ready, using attack animation
        }
        else
            status.SetState(this);

        castFramesCount++;
    }

    public override void UpdateAfterCollisionCheck(PlayerCollider collider, PlayerStatus status, PlayerInput input)
    {

    }

}
