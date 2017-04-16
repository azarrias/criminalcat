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
	public void CalculateSpeed( PlayerInput input, PlayerStatus status )
	{
        // horizontal speed calculations
        //speed.x = Mathf.SmoothDamp(speed.x, input.newInput.GetHorizontalInput()* moveSpeed * Time.fixedDeltaTime, ref xCurrentSpeed, xSpeedChangeSpeed);
        if (status.justHit)
            speed.x = status.facingRight ? - hitRecoil*Time.fixedDeltaTime : hitRecoil * Time.fixedDeltaTime;
        else if (status.CanMoveHorizontally() == true)
            speed.x = input.newInput.GetHorizontalInput() * moveSpeed * Time.fixedDeltaTime;
        else
            speed.x = 0;

        // vertical speed calculations
        if (status.IsJump())
        {
            speed.y += (jumpSpeed - gravity * Time.fixedDeltaTime) * Time.fixedDeltaTime;
        }
        else if (status.IsFall() || status.IsFallCloud() || status.IsHit() || status.IsAttack())  
        {
            //speed.y += -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            speed.y += -gravity*0.8f * Time.fixedDeltaTime * Time.fixedDeltaTime;
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
}
