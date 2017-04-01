using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {

	int framesToJumpMax;
	int framesToJumpMin;
	int framesToJumpCount;
    bool jumpAvailable;

    int framesToFallThroughCloudPlatforms;
    int framesToFallThroughCloudPlatformsCount;

    bool facingRight;
    [HideInInspector]
    public bool climbLadderAvailable;

    pStatus oldStatus;
    [HideInInspector]
	public pStatus newStatus;

	void Start (){
		framesToJumpCount = 0;
        framesToFallThroughCloudPlatformsCount = 0;

		framesToJumpMax = CalculateFramesFromTime(GetComponent<PlayerMove> ().timeToJumpApex);
		framesToJumpMin = (int)(framesToJumpMax / 4);

        framesToFallThroughCloudPlatforms = CalculateFramesFromTime(GetComponent<PlayerMove>().timeToFallThroughCloudPlatforms);

        jumpAvailable = false;
		oldStatus.Reset (); newStatus.Reset ();
		facingRight = true;
        climbLadderAvailable = false;

    }

    public void statusUpdateAfterInput(PlayerInput input) {
        if ((input.newInput.GetHorizontalInput() < 0 && facingRight) || (input.newInput.GetHorizontalInput() > 0 && !facingRight)) {
            Flip();
        }

        //vertical status
        oldStatus.CopyStatusFrom(newStatus);
        if (input.newInput.GetVerticalInput() != 0 && climbLadderAvailable)
        {
            newStatus.SetClimbingLadder();
        }
        else
        {
            if (oldStatus.IsJump())
            {
                if (input.newInput.GetJumpButtonHeld() && (framesToJumpCount <= framesToJumpMax))
                {
                    newStatus.SetJump();
                    framesToJumpCount++;
                }
                else
                {
                    if (framesToJumpCount > framesToJumpMin)
                    {
                        newStatus.SetFall();
                        framesToJumpCount = 0;
                    }
                    else
                    {
                        newStatus.SetJump();
                        framesToJumpCount++;
                    }
                }
                return;
            }

            if (oldStatus.IsGround())
            {
                if (!input.newInput.GetJumpButtonHeld())
                {
                    jumpAvailable = true;
                }
                if (jumpAvailable && input.newInput.GetJumpButtonHeld())
                {

                    if (input.newInput.GetVerticalInput() < 0 && GetComponent<PlayerCollider>().PlayerAboveCloudPlatform() == true)
                    {
                        newStatus.SetFallThroughCloudPlatform();
                        framesToFallThroughCloudPlatformsCount = 1;
                    }
                    else
                    {
                        newStatus.SetJump();
                        framesToJumpCount = 1;
                    }
                    jumpAvailable = false;
                    return;
                }
            }

            if (oldStatus.IsFall())
            {
                newStatus.SetFall();
                return;
            }

            if (oldStatus.IsFallThroughCloudPlatform())
            {
                if (framesToFallThroughCloudPlatformsCount <= framesToFallThroughCloudPlatforms)
                {
                    newStatus.SetFallThroughCloudPlatform();
                    framesToFallThroughCloudPlatformsCount++;
                }
                else
                {
                    newStatus.SetFall();
                    framesToFallThroughCloudPlatformsCount = 0;
                }
                return;
            }

            if (oldStatus.IsClimbingLadder())
            {
                if (input.newInput.GetHorizontalInput() != 0 || climbLadderAvailable == false)
                    newStatus.SetFall();
                else
                    newStatus.IsClimbingLadder();
            }
        }
	}

	public void statusUpdateAfterCollisionCheck( PlayerCollider collider)
	{
		if (newStatus.IsGround () && !collider.collisions.below){
			newStatus.SetFall ();
			return;
		}
		if (newStatus.IsFall () && collider.collisions.below) {
			newStatus.SetGround ();
			return;
		}
	
		if (newStatus.IsJump () && collider.collisions.above) {
			framesToJumpCount = 0;
			newStatus.SetFall ();
		}
        if (newStatus.IsFallThroughCloudPlatform()) // no vertical collisions here
        {
            newStatus.IsFallThroughCloudPlatform();
            return;
        }
        if(newStatus.IsClimbingLadder() && collider.collisions.below)
        {
            newStatus.IsGround();
            return;
        }
	}

	int CalculateFramesFromTime( float time )
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
    static readonly int FALL_THROUGH_CLOUD_PLATFORM = 0x0008;
    static readonly int CLIMBING_LADDER = 0x0010;

    public struct pStatus {

        int statusMask;

        public void Reset() { statusMask = GROUND; }

        //bit-wise functions
        //public void SetMask( int mask ){ statusMask = statusMask & mask;}
        public void SetMask(int mask) { statusMask = mask; }
        public bool TestMask(int mask) { return (statusMask & mask) == mask; }

        //readable functions
        //getters
        public bool IsGround() { return TestMask(GROUND); }
        public bool IsJump() { return TestMask(JUMP); }
        public bool IsFall() { return TestMask(FALL); }
        public bool IsFallThroughCloudPlatform() { return TestMask(FALL_THROUGH_CLOUD_PLATFORM); }
        public bool IsClimbingLadder() { return TestMask(CLIMBING_LADDER); }

		// setters
		public void SetGround(){ SetMask(GROUND);}
		public void SetJump(){ SetMask(JUMP);}
		public void SetFall(){ SetMask(FALL);}
        public void SetFallThroughCloudPlatform() { SetMask(FALL_THROUGH_CLOUD_PLATFORM); }
        public void SetClimbingLadder() { SetMask(CLIMBING_LADDER); }

		public void CopyStatusFrom( pStatus from)
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
