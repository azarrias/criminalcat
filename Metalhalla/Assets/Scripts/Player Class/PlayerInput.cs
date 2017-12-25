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
        private float leftTriggerInput;
        private float rightTriggerInput;
        private bool leftTriggerDown;
        private bool rightTriggerDown;

        private bool tauntButtonDown;
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
            leftTriggerInput = 0f;
            rightTriggerInput = 0f;
            leftTriggerDown = false;
            rightTriggerDown = false;

            tauntButtonDown = false;
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
        public void SetLeftTriggerInput( float value) { leftTriggerInput = value; }
        public void SetRightTriggerInput( float value ) { rightTriggerInput = value; }
        public void SetLeftTriggerDown( bool value) { leftTriggerDown = value;  }
        public void SetRightTriggerDown( bool value ) { rightTriggerDown = value; }

        public void SetTauntButtonDown( bool value) { tauntButtonDown = value; }
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
        public float GetLeftTriggerInput() { return leftTriggerInput; }
        public float GetRightTriggerInput() { return rightTriggerInput; }
        public bool GetLeftTriggerDown() { return leftTriggerDown; }
        public bool GetRightTriggerDown() { return rightTriggerDown; }

        public bool GetTauntButtonDown() { return tauntButtonDown; }
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
            leftTriggerInput = from.leftTriggerInput;
            rightTriggerInput = from.rightTriggerInput;
            leftTriggerDown = from.leftTriggerDown;
            rightTriggerDown = from.rightTriggerDown;

            tauntButtonDown = from.tauntButtonDown;
            horizontalDoubleTap = from.horizontalDoubleTap;
        }
	};

    public pInput newInput;
    [HideInInspector]
    public pInput oldInput;

    GUIManager guiManager; 

    public int doubleTapFramesMax = 12;
    int doubleTapFramesCount; 

    enum doubleTap { IDLE, FIRST_TAP, FIRST_RELEASED, SECOND_TAP};
    doubleTap hDoubleTap;
    bool facingRightTap; 

    private void Start()
    {
        StartPlayerInput();
    }

    public virtual void GetInput()
	{
		oldInput.CopyInputFrom (newInput);  //make a savestate from last input
    // OLD CONTROL MAPPING
        /*
        newInput.SetHorizontalInput(Input.GetAxis("DPadHorizontal"));
        newInput.SetVerticalInput(Input.GetAxis("DPadVertical"));
        newInput.SetJumpButtonDown(Input.GetButtonDown("ButtonA"));
        newInput.SetJumpButtonHeld(Input.GetButton("ButtonA"));
        newInput.SetAttackButtonDown(Input.GetButtonDown("ButtonX"));
        newInput.SetDashButtonDown(Input.GetButtonDown("ButtonLB"));
        newInput.SetDefenseButtonDown(Input.GetButtonDown("ButtonRB"));
        newInput.SetDefenseButtonHeld(Input.GetButton("ButtonRB"));
        newInput.SetCastButtonDown(Input.GetButtonDown("ButtonB"));
        newInput.SetContextButtonDown(Input.GetButtonDown("ButtonY"));
        newInput.SetLeftTriggerInput(Input.GetAxis("LeftTrigger"));
        newInput.SetRightTriggerInput(Input.GetAxis("RightTrigger"));

        newInput.SetHorizontalDoubleTap(CheckHorizontalDoubleTap());
        */
    // NEW CONTROL MAPPING
        newInput.SetHorizontalInput(Input.GetAxis("DPadHorizontal"));
        newInput.SetVerticalInput(Input.GetAxis("DPadVertical"));
        newInput.SetJumpButtonDown(Input.GetButtonDown("ButtonA"));
        newInput.SetJumpButtonHeld(Input.GetButton("ButtonA"));
        newInput.SetAttackButtonDown(Input.GetButtonDown("ButtonX"));
        newInput.SetDashButtonDown(Input.GetButtonDown("ButtonB"));
        newInput.SetDefenseButtonDown(Input.GetButtonDown("ButtonRB") || Input.GetButtonDown("ButtonLB"));
        newInput.SetDefenseButtonHeld(Input.GetButton("ButtonRB") || Input.GetButton("ButtonLB"));
        newInput.SetContextButtonDown(Input.GetButtonDown("ButtonY"));

        newInput.SetLeftTriggerInput(Input.GetAxis("LeftTrigger"));
        newInput.SetRightTriggerInput(Input.GetAxis("RightTrigger"));
        newInput.SetLeftTriggerDown(!oldInput.GetLeftTriggerDown() && oldInput.GetLeftTriggerInput() == 0 && newInput.GetLeftTriggerInput() > 0);
        newInput.SetRightTriggerDown(!oldInput.GetRightTriggerDown() && oldInput.GetRightTriggerInput() == 0 && newInput.GetRightTriggerInput() > 0);

        newInput.SetTauntButtonDown(Input.GetButton("ButtonSelect"));
        newInput.SetCastButtonDown( newInput.GetLeftTriggerDown() || newInput.GetRightTriggerDown() );
    }

    private bool CheckHorizontalDoubleTap()
    {
        // mod: always return false and disable double tapping
        bool ret = false;

        /*
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
        */
        return ret; 
    }

    public void StartPlayerInput()
    {
        GameObject guiObject = GameObject.Find("GUI");
        if (guiObject)
            guiManager = guiObject.GetComponent<GUIManager>();
        doubleTapFramesCount = 0;
        hDoubleTap = doubleTap.IDLE;
    }
}
