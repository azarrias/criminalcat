using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	public float moveSpeed = 5;
	public float gravity = 40f;
	public float jumpSpeed = 1.8f;
	public float timeToJumpApex = 0.2f;

	public float xSpeedChangeSpeed = 0.99f;

    public float timeToFallThroughCloudPlatforms = 0.1f;

    [HideInInspector]
	public Vector3 speed;
	[HideInInspector]
	public float xCurrentSpeed;
	public void CalculateSpeed( PlayerInput input, PlayerStatus status )
	{
        //speed.x = Mathf.SmoothDamp(speed.x, input.newInput.GetHorizontalInput()* moveSpeed * Time.fixedDeltaTime, ref xCurrentSpeed, xSpeedChangeSpeed);
        speed.x = input.newInput.GetHorizontalInput() * moveSpeed * Time.fixedDeltaTime;

        // vertical one :)
        if (status.newStatus.IsGround()){
			speed.y = -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
		}
		if (status.newStatus.IsJump ()) {
			speed.y += (jumpSpeed - gravity * Time.fixedDeltaTime) * Time.fixedDeltaTime;
		}
		if (status.newStatus.IsFall () || status.newStatus.IsFallThroughCloudPlatform() ) {
			speed.y += -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
		}
        if (status.newStatus.IsClimbingLadder() )
        {
            speed.y = moveSpeed * Time.fixedDeltaTime * input.newInput.GetVerticalInput();
        }
	}

	public void Move(){
		transform.Translate (speed);
	}
}
