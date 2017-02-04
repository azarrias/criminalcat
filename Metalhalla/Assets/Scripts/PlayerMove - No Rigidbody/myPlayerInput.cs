using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class myPlayerInput : MonoBehaviour {

	//*********************************************
	// INPUT HANDLING
	//*********************************************
	public struct PlayerInput
	{
		private float horizontalInput;
		private float verticalInput;
		private bool jumpButtonDown;
		private bool jumpButtonHeld;

		public void Reset(){
			horizontalInput = 0f;
			verticalInput = 0f;
			jumpButtonDown = false;
			jumpButtonHeld = false;
		}
		public void SetHorizontalInput(float value)	{ horizontalInput = value; }
		public void SetVerticalInput( float value ) { verticalInput = value; }
		public void SetJumpButtonDown( bool value ) { jumpButtonDown = value; }
		public void SetJumpButtonHeld (bool value) { jumpButtonHeld = value;}

		public float GetHorizontalInput(){ return horizontalInput;}
		public float GetVerticalInput(){   return verticalInput;}
		public bool GetJumpButtonDown(){  return jumpButtonDown;}
		public bool  GetJumpButtonHeld(){  return jumpButtonHeld;}

		public void CopyInputFrom( PlayerInput from)
		{
			horizontalInput = from.horizontalInput;
			verticalInput = from.verticalInput;
			jumpButtonDown = from.jumpButtonDown;
			jumpButtonHeld = from.jumpButtonHeld;
		}
	};

	public PlayerInput newInput;
	PlayerInput oldInput;

	public void GetInput()
	{
		oldInput.CopyInputFrom (newInput);	//make a savestate from last input

		#if UNITY_EDITOR

		newInput.SetHorizontalInput( Input.GetAxis("Horizontal"));
		newInput.SetVerticalInput( Input.GetAxis("Vertical"));
		newInput.SetJumpButtonDown(Input.GetButtonDown("Jump"));
		newInput.SetJumpButtonHeld(Input.GetButton ("Jump") );

		#elif UNITY_ANDROID

		newInput.SetHorizontalInput( CrossPlatformInputManager.GetAxis("Horizontal"));
		newInput.SetVerticalInput( CrossPlatformInputManager.GetAxis("Vertical"));
		newInput.SetJumpButtonDown(CrossPlatformInputManager.GetButtonDown("Jump"));
		newInput.SetJumpButtonHeld(CrossPlatformInputManager.GetButton ("Jump") );

		#endif
	}
}
