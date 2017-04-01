﻿using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	//*********************************************
	// INPUT HANDLING
	//*********************************************
	public struct pInput
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

		public void CopyInputFrom( pInput from)
		{
			horizontalInput = from.horizontalInput;
			verticalInput = from.verticalInput;
			jumpButtonDown = from.jumpButtonDown;
			jumpButtonHeld = from.jumpButtonHeld;
		}
	};

	public pInput newInput;
    pInput oldInput;

	public void GetInput()
	{
		oldInput.CopyInputFrom (newInput);	//make a savestate from last input

        // update with new inputs / changes in axis in the controller
		newInput.SetHorizontalInput( Input.GetAxis("DPadHorizontal"));
		newInput.SetVerticalInput( Input.GetAxis("DPadVertical"));
		newInput.SetJumpButtonDown(Input.GetButtonDown("Jump"));
		newInput.SetJumpButtonHeld(Input.GetButton ("Jump") );
	}
}
