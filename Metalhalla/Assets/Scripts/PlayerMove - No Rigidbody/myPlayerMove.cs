using UnityEngine;
using System.Collections;

public class myPlayerMove : MonoBehaviour {

	public float moveSpeed = 4;
	public float gravity = 20f;
	public float jumpSpeed = 2f;
	public float timeToJumpApex = 0.4f;

	public float xSpeedChangeSpeed = 0.1f;

    public float timeToFallThroughCloudPlatforms = 0.1f;

    [HideInInspector]
	public Vector3 speed;
	[HideInInspector]
	public float xCurrentSpeed;
	public void CalculateSpeed( myPlayerInput input, myPlayerStatus status )
	{
		//aqui en función del input y del estado, calculo unas velocidades u otras :)
		//	speed.x = input.newInput.GetHorizontalInput() * moveSpeed * Time.fixedDeltaTime;
		speed.x = Mathf.SmoothDamp(speed.x, input.newInput.GetHorizontalInput()* moveSpeed * Time.fixedDeltaTime, ref xCurrentSpeed, xSpeedChangeSpeed);

		// vertical one :)
		if (status.newStatus.IsGround()){
			speed.y = -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
			//speed.y = -0.2f;
		}
		if (status.newStatus.IsJump ()) {
			speed.y += (jumpSpeed - gravity * Time.fixedDeltaTime) * Time.fixedDeltaTime;
		}
		if (status.newStatus.IsFall () || status.newStatus.IsFallThroughCloudPlatform() ) {
			speed.y += -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
		}
	}

	public void Move(){
		transform.Translate (speed);
	}
}
