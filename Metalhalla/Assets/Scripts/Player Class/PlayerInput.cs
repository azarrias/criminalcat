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
        private bool dashButtonDown; 
        private bool defenseButtonDown;
        private bool defenseButtonHeld; 
        private bool castButtonDown;
        private bool contextButtonDown;

        //double tap control
        private bool horizontalDoubleTap;


		public void Reset(){
			horizontalInput = 0f;
			verticalInput = 0f;
			jumpButtonDown = false;
			jumpButtonHeld = false;
            attackButtonDown = false;
            dashButtonDown = false; 
            defenseButtonDown = false;
            defenseButtonHeld = false; 
            castButtonDown = false;
            contextButtonDown = false;

            horizontalDoubleTap = false; 
        }

        public void SetHorizontalInput(float value)	{ horizontalInput = value; }
        public void SetVerticalInput( float value ) { verticalInput = value; }
        public void SetJumpButtonDown( bool value ) { jumpButtonDown = value; }
        public void SetJumpButtonHeld (bool value) { jumpButtonHeld = value;}
        public void SetAttackButtonDown(bool value) { attackButtonDown = value; }
        public void SetDashButtonDown(bool value) { dashButtonDown = value;  }
        public void SetDefenseButtonDown(bool value) { defenseButtonDown = value; }
        public void SetDefenseButtonHeld(bool value) { defenseButtonHeld = value;  }
        public void SetCastButtonDown(bool value) { castButtonDown = value; }
        public void SetContextButtonDown(bool value) { contextButtonDown = value;  }

        public void SetHorizontalDoubleTap(bool value) { horizontalDoubleTap = value; }

		public float GetHorizontalInput(){ return horizontalInput;}
		public float GetVerticalInput(){   return verticalInput;}
		public bool GetJumpButtonDown(){  return jumpButtonDown;}
		public bool  GetJumpButtonHeld(){  return jumpButtonHeld;}
        public bool GetAttackButtonDown() { return attackButtonDown; }
        public bool GetDashButtonDown() { return dashButtonDown; }
        public bool GetDefenseButtonDown() { return defenseButtonDown; }
        public bool GetDefenseButtonHeld() { return defenseButtonHeld; }
        public bool GetCastButtonDown() { return castButtonDown; }
        public bool GetContextButtonDown() { return contextButtonDown; }

        public bool GetHorizontalDoubleTap() { return horizontalDoubleTap; }

		public void CopyInputFrom( pInput from)
		{
			horizontalInput = from.horizontalInput;
			verticalInput = from.verticalInput;
			jumpButtonDown = from.jumpButtonDown;
			jumpButtonHeld = from.jumpButtonHeld;
            attackButtonDown = from.attackButtonDown;
            dashButtonDown = from.dashButtonDown;
            defenseButtonDown = from.defenseButtonDown;
            defenseButtonHeld = from.defenseButtonHeld;
            castButtonDown = from.castButtonDown;
            contextButtonDown = from.contextButtonDown;

            horizontalDoubleTap = from.horizontalDoubleTap;

        }
	};

    public pInput newInput;
    pInput oldInput;
    
    //mod for double tap input
    public int doubleTapFramesMax = 12;
    int doubleTapFramesCount; 

    enum doubleTap { IDLE, FIRST_TAP, FIRST_RELEASED, SECOND_TAP};
    doubleTap hDoubleTap;
    bool facingRightTap; 

    public PlayerInput()
    {
        doubleTapFramesCount = 0; 
        hDoubleTap = doubleTap.IDLE;
    }
    // end mod for double tap input

	public void GetInput()
	{
		oldInput.CopyInputFrom (newInput);	//make a savestate from last input

		newInput.SetHorizontalInput( Input.GetAxis("DPadHorizontal"));
		newInput.SetVerticalInput( Input.GetAxis("DPadVertical"));
		newInput.SetJumpButtonDown(Input.GetButtonDown("Jump"));
		newInput.SetJumpButtonHeld(Input.GetButton ("Jump") );
        newInput.SetAttackButtonDown(Input.GetButtonDown("Attack"));
        newInput.SetDashButtonDown(Input.GetButtonDown("Dash"));
        newInput.SetDefenseButtonDown(Input.GetButtonDown("Defense"));
        newInput.SetDefenseButtonHeld(Input.GetButton("Defense"));
        newInput.SetCastButtonDown(Input.GetButtonDown("Cast"));
        newInput.SetContextButtonDown(Input.GetButtonDown("Context"));

        newInput.SetHorizontalDoubleTap(CheckHorizontalDoubleTap());
	}

    private bool CheckHorizontalDoubleTap()
    {
        bool ret = false; 
        switch( hDoubleTap)
        {
            case doubleTap.IDLE:
                doubleTapFramesCount = 0; 
                if (oldInput.GetHorizontalInput() == 0 && newInput.GetHorizontalInput() != 0)
                {
                    hDoubleTap = doubleTap.FIRST_TAP;
                    facingRightTap = newInput.GetHorizontalInput() > 0 ? true : false; 
                }
            break;

            case doubleTap.FIRST_TAP:
                doubleTapFramesCount++;
                if (doubleTapFramesCount >= doubleTapFramesMax)
                    hDoubleTap = doubleTap.IDLE;
                else if (newInput.GetHorizontalInput() == 0)
                    hDoubleTap = doubleTap.FIRST_RELEASED;
                break;

            case doubleTap.FIRST_RELEASED:
                doubleTapFramesCount++;
                if (doubleTapFramesCount >= doubleTapFramesMax)
                    hDoubleTap = doubleTap.IDLE;
                else if ((newInput.GetHorizontalInput() > 0 && facingRightTap) || (newInput.GetHorizontalInput() < 0 && !facingRightTap))
                    hDoubleTap = doubleTap.SECOND_TAP;
                break;

            case doubleTap.SECOND_TAP:
                ret = true;
                hDoubleTap = doubleTap.IDLE;
                break; 
        }
        return ret; 
    }
}
