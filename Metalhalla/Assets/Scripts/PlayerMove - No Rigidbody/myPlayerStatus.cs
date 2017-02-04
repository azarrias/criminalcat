using UnityEngine;
using System.Collections;

public class myPlayerStatus : MonoBehaviour {

	public int framesToJumpInAdvance = 5;
	public int framesToJumpInDelay = 5;
	int framesToJumpMax;
	int framesToJumpMin;

	int framesInAdvanceCount;
	int framesInDelayCount;
	int framesToJumpCount;

	bool jumpInAdvanceAvailable;
	bool jumpInDelayAvailable;
	bool jumpAvailable;

	bool facingRight;

	PlayerStatus oldStatus;
	public PlayerStatus newStatus;

	void Start (){
		framesToJumpCount = 0;
		framesInDelayCount = 0;
		framesInAdvanceCount = 0;
		framesToJumpMax = CalculateFramesToJumpMax (GetComponent<myPlayerMove> ().timeToJumpApex);
		framesToJumpMin = (int)(framesToJumpMax / 4);
		jumpInAdvanceAvailable = false;
		jumpInDelayAvailable = false;
		jumpAvailable = false;
		oldStatus.Reset (); newStatus.Reset ();
		facingRight = true;
	}

	public void statusUpdateAfterInput (myPlayerInput input){
		if ((input.newInput.GetHorizontalInput () < 0 && facingRight) || (input.newInput.GetHorizontalInput () > 0 && !facingRight)) {
			Flip ();
		}

			//vertical status
		oldStatus.CopyStatusFrom (newStatus);
	//	print ("old.ground : " + oldStatus.IsGround () + " | old.jump : " + oldStatus.IsJump () + " | old.fall : " + oldStatus.IsFall ());
		if (oldStatus.IsJump ()) {
			if (input.newInput.GetJumpButtonHeld () && (framesToJumpCount <= framesToJumpMax)) {
				newStatus.SetJump ();
				framesToJumpCount++;
			} else {
				if (framesToJumpCount > framesToJumpMin) {
					newStatus.SetFall ();
					framesToJumpCount = 0;
				} else {
					newStatus.SetJump ();
					framesToJumpCount++;
				}	
			}
			return;
		}

		/*
		if (oldStatus.IsGround ()) {
			if (input.newInput.GetJumpButtonDown ()) {
				newStatus.SetJump ();
				framesToJumpCount = 1;
				return;
			}
			if (jumpInAdvanceAvailable && input.newInput.GetJumpButtonHeld ()) {
				newStatus.SetJump ();
				framesToJumpCount = 1;
				return;
			}
		}
		*/
		if (oldStatus.IsGround ()) {
			if (!input.newInput.GetJumpButtonHeld ()) {
				jumpAvailable = true;
			}
			if ((jumpInAdvanceAvailable && input.newInput.GetJumpButtonHeld ()) || input.newInput.GetJumpButtonDown () ) {
				newStatus.SetJump ();
				framesToJumpCount = 1;
				jumpAvailable = false;
				return;
			}
			if (jumpAvailable && input.newInput.GetJumpButtonHeld ()) {
				newStatus.SetJump ();
				framesToJumpCount = 1;
				jumpAvailable = false;
				return;
			}
		}

		if (oldStatus.IsFall ()) {
			// specialConditionsJump check!
			newStatus.SetFall();
			if (jumpInDelayAvailable) {
				if (input.newInput.GetJumpButtonDown ()) {
					newStatus.SetJump ();
					framesToJumpCount++;
					jumpInDelayAvailable = false;
					framesInDelayCount = 0;
				} else {
					framesInDelayCount++;
					if (framesInDelayCount > framesToJumpInDelay) {
						jumpInDelayAvailable = false;
						framesInDelayCount = 0;
					}
				}
			} else if (jumpInAdvanceAvailable) {
				if (input.newInput.GetJumpButtonHeld ()) {
					framesInAdvanceCount++;
					if (framesInAdvanceCount > framesToJumpInAdvance){
						jumpInAdvanceAvailable = false;
						framesInAdvanceCount = 0;
					}
				} else {
					jumpInAdvanceAvailable = false;
				}
			} else {
				if (input.newInput.GetJumpButtonDown ()) {
					jumpInAdvanceAvailable = true;
					framesInAdvanceCount++;
				}
			}
			return;
		}
	}

	public void statusUpdateAfterCollisionCheck( myPlayerCollider collider)
	{
		if (newStatus.IsGround () && !collider.collisions.below){
			newStatus.SetFall ();
			jumpInDelayAvailable = true;
			print (1);
			return;
		}
		if (newStatus.IsFall () && collider.collisions.below) {
			newStatus.SetGround ();
			jumpInDelayAvailable = false;
			return;
		}
	
		if (newStatus.IsJump () && collider.collisions.above) {
			framesToJumpCount = 0;
			newStatus.SetFall ();
		}
	}

	int CalculateFramesToJumpMax( float time )
	{
		return (int) (time/ Time.fixedDeltaTime);
	}

	/************ GOLD *************************/
	/* old definition

	//status mask values
	static readonly int HMASK = 0x00FF;
	static readonly int IDLE = 0x0001;
	static readonly int WALK = 0x0002;
	static readonly int RUN  = 0x0004;
	//static readonly int BRAKE = 0x0008;

	// status mask vertical values
	static readonly int VMASK = 0xFF00;
	static readonly int GROUND = 0x0100;
	static readonly int JUMP = 0x0200;
	static readonly int FALL = 0x0400;

	struct PlayerStatus{

		int statusMask;

		public void Reset(){ statusMask = IDLE | GROUND;}

		//bit-wise functions
		public void SetVerticalMask( int mask )	 {	statusMask = statusMask & HMASK;	statusMask = statusMask | mask;	}
		public void SetHorizontalMask( int mask ){	statusMask = statusMask & VMASK;	statusMask = statusMask | mask; }
		public bool TestMask( int mask ){ return (statusMask & mask) ==  mask;}

		//readable functions
		//getters
		public bool GetIdle(){ return TestMask(IDLE);}
		public bool GetWalk(){ return TestMask(WALK);}
		public bool GetRun(){ return TestMask(RUN);}
		//	public bool GetBrake(){ return TestMask(BRAKE);}
		public bool GetGround(){ return TestMask(GROUND);}
		public bool GetJump(){ return TestMask(JUMP);}
		public bool GetFall(){ return TestMask(FALL);}

		// setters
		public void SetIdle(){ SetHorizontalMask(IDLE);}
		public void SetWalk(){ SetHorizontalMask(WALK);}
		public void SetRun(){ SetHorizontalMask(RUN);}
		//	public void SetBrake(){ SetHorizontalMask(BRAKE);}
		public void SetGround(){ SetVerticalMask(GROUND);}
		public void SetJump(){ SetVerticalMask(JUMP);}
		public void SetFall(){ SetVerticalMask(FALL);}

		void CopyStatusFrom( PlayerStatus from)
		{
			statusMask = from.statusMask;
		}
	};

	*/

	//new definition
	//status mask values
	// status mask vertical values
	static readonly int GROUND = 0x0001;
	static readonly int JUMP = 0x0002;
	static readonly int FALL = 0x0004;

	public struct PlayerStatus{

		int statusMask;

		public void Reset(){ statusMask = GROUND;}

		//bit-wise functions
		//public void SetMask( int mask ){ statusMask = statusMask & mask;}
		public void SetMask( int mask ){ statusMask = mask;}
		public bool TestMask( int mask ){ return (statusMask & mask) ==  mask;}

		//readable functions
		//getters
		public bool IsGround(){ return TestMask(GROUND);}
		public bool IsJump(){ return TestMask(JUMP);}
		public bool IsFall(){ return TestMask(FALL);}

		// setters
		public void SetGround(){ SetMask(GROUND);}
		public void SetJump(){ SetMask(JUMP);}
		public void SetFall(){ SetMask(FALL);}

		public void CopyStatusFrom( PlayerStatus from)
		{
			statusMask = from.statusMask;
		}
	};

	public void Flip(){
		facingRight = !facingRight;
		Vector3 tmp = transform.localScale;
		tmp.x *= -1;
		transform.localScale = tmp;

	}

}
