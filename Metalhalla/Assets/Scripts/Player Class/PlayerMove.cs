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

    [Header("Durations")]
    public float attackDuration = 0.400f; 

    [HideInInspector]
	public Vector3 speed;
	[HideInInspector]
	public float xCurrentSpeed;
	public void CalculateSpeed( PlayerInput input, PlayerStatus status )
	{
        //speed.x = Mathf.SmoothDamp(speed.x, input.newInput.GetHorizontalInput()* moveSpeed * Time.fixedDeltaTime, ref xCurrentSpeed, xSpeedChangeSpeed);
        speed.x = input.newInput.GetHorizontalInput() * moveSpeed * Time.fixedDeltaTime;

        
        if (status.currentState == PlayerStatus.idle) { 
			speed.y = -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
		}
		if (status.currentState == PlayerStatus.jump) {
			speed.y += (jumpSpeed - gravity * Time.fixedDeltaTime) * Time.fixedDeltaTime;
		}
		if (status.currentState == PlayerStatus.fall ) {
			speed.y += -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
		}
        if (status.currentState == PlayerStatus.climb)
        {
            speed.y = moveSpeed * Time.fixedDeltaTime * input.newInput.GetVerticalInput(); 
        }
    
	}

	public void Move(){
		transform.Translate (speed);
	}
}
