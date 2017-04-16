using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

    [Header("Basic move, jump & gravity Setup")]
    public float moveSpeed = 5;
	public float gravity = 40f;
	public float jumpSpeed = 1.8f;
	public float timeToJumpApex = 0.2f;

	public float xSpeedChangeSpeed = 0.99f;

    public float timeToFallThroughCloudPlatforms = 0.1f;

    [Header("Other move constraints")]
    public float hitRecoil = 0.2f;

    [HideInInspector]
	public Vector3 speed;
	[HideInInspector]
	public float xCurrentSpeed;

    private float[] jumpSpeeds;



    private void Start()
    {
        CalculateJumpFramesSpeed();
    }

    public void CalculateSpeed( PlayerInput input, PlayerStatus status, PlayerCollider collider )
	{
        // horizontal speed calculations
        if (status.justHit == true )
            speed.x = status.facingRight ? - hitRecoil*Time.fixedDeltaTime : hitRecoil * Time.fixedDeltaTime;
        else if ( AllowHorizontalInput(status, collider) == true )
            speed.x = input.newInput.GetHorizontalInput() * moveSpeed * Time.fixedDeltaTime;
        else
            speed.x = 0;

        // vertical speed calculations
        if (status.IsJump())
        {
            speed.y = jumpSpeeds[status.jumpFrames];
            Debug.Log("jumpFrames = " + status.jumpFrames);
            //speed.y += (jumpSpeed - gravity * Time.fixedDeltaTime) * Time.fixedDeltaTime;
        }
        else if (status.IsFall() || status.IsFallCloud() || status.IsHit() || status.IsAttack())  
        {
            //speed.y += -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            /*if (status.WasJump())
                speed.y = 0;
            else*/
                speed.y += -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        }
        else if (status.IsClimb())
        {
            speed.y = moveSpeed * Time.fixedDeltaTime * input.newInput.GetVerticalInput();
        }
        else 
            speed.y = -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;

    }

	public void Move(){
        transform.Translate (speed);
	}

    public bool AllowHorizontalInput( PlayerStatus status, PlayerCollider collider)
    {
        if (collider.IsGrounded() == false)
            return true;
        if (status.IsIdle() || status.IsWalk() )
            return true;
        return false;
    }

    int CalculateFramesFromTime(float time)
    {
        return (int)(time / Time.fixedDeltaTime);
    }

    void CalculateJumpFramesSpeed()
    {
        int maxFrame = CalculateFramesFromTime(timeToJumpApex);
        //float initialJumpSpeed = gravity * timeToJumpApex; 
        float initialJumpSpeed = jumpSpeed;

        jumpSpeeds = new float[maxFrame+1];

        jumpSpeeds[0] = 0;
        for (int i = 1; i < maxFrame+1; i++)
        {
            jumpSpeeds[i] = initialJumpSpeed * i * Time.fixedDeltaTime - 0.5f * gravity * i * i * Time.fixedDeltaTime * Time.fixedDeltaTime;
            jumpSpeeds[i] -= jumpSpeeds[i - 1];
        }
    }
}
