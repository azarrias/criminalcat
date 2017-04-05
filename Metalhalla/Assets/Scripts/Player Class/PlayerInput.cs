using UnityEngine;
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
        private bool attackButtonDown;
        private bool defenseButtonDown;
        private bool defenseButtonHeld; 
        private bool castButtonDown;
        private bool contextButtonDown;

		public void Reset(){
			horizontalInput = 0f;
			verticalInput = 0f;
			jumpButtonDown = false;
			jumpButtonHeld = false;
            attackButtonDown = false;
            defenseButtonDown = false;
            defenseButtonHeld = false; 
            castButtonDown = false;
            contextButtonDown = false;
        }

		public void SetHorizontalInput(float value)	{ horizontalInput = value; }
		public void SetVerticalInput( float value ) { verticalInput = value; }
		public void SetJumpButtonDown( bool value ) { jumpButtonDown = value; }
		public void SetJumpButtonHeld (bool value) { jumpButtonHeld = value;}
        public void SetAttackButtonDown(bool value) { attackButtonDown = value; }
        public void SetDefenseButtonDown(bool value) { defenseButtonDown = value; }
        public void SetDefenseButtonHeld(bool value) { defenseButtonHeld = value;  }
        public void SetCastButtonDown(bool value) { castButtonDown = value; }
        public void SetContextButtonDown(bool value) { contextButtonDown = value;  }

		public float GetHorizontalInput(){ return horizontalInput;}
		public float GetVerticalInput(){   return verticalInput;}
		public bool GetJumpButtonDown(){  return jumpButtonDown;}
		public bool  GetJumpButtonHeld(){  return jumpButtonHeld;}
        public bool GetAttackButtonDown() { return attackButtonDown; }
        public bool GetDefenseButtonDown() { return defenseButtonDown; }
        public bool GetDefenseButtonHeld() { return defenseButtonHeld; }
        public bool GetCastButtonDown() { return castButtonDown; }
        public bool GetContextButtonDown() { return contextButtonDown; }

		public void CopyInputFrom( pInput from)
		{
			horizontalInput = from.horizontalInput;
			verticalInput = from.verticalInput;
			jumpButtonDown = from.jumpButtonDown;
			jumpButtonHeld = from.jumpButtonHeld;
            attackButtonDown = from.attackButtonDown;
            defenseButtonDown = from.defenseButtonDown;
            defenseButtonHeld = from.defenseButtonHeld;
            castButtonDown = from.castButtonDown;
            contextButtonDown = from.contextButtonDown; 
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
        newInput.SetAttackButtonDown(Input.GetButtonDown("Attack"));
        newInput.SetDefenseButtonDown(Input.GetButtonDown("Defense"));
        newInput.SetDefenseButtonHeld(Input.GetButton("Defense"));
        newInput.SetCastButtonDown(Input.GetButtonDown("Cast"));
        newInput.SetContextButtonDown(Input.GetButtonDown("Context")); 

        
	}
}
