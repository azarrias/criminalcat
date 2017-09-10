using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerInput))]
[RequireComponent (typeof(PlayerStatus))]
[RequireComponent (typeof(PlayerMove))]
[RequireComponent (typeof(PlayerCollider))]

public class PlayerController : MonoBehaviour {

    public bool useInputAI = false;

	PlayerInput 	 playerInput;
	PlayerStatus     playerStatus;
	PlayerMove  	 playerMove;
	PlayerCollider   playerCollider;
	Animator		 playerAnimator;

	void Start() {
        if (useInputAI)
            playerInput = GetComponent<PlayerInputAI>();
        else
            playerInput = GetComponent<PlayerInput> ();
        playerStatus = GetComponent<PlayerStatus> ();
		playerMove = GetComponent<PlayerMove> ();
		playerCollider = GetComponent<PlayerCollider> ();
		playerAnimator = GetComponent<Animator> ();
	}

	void Update () {
		playerInput.GetInput ();
    }
	void FixedUpdate (){
		
		playerStatus.statusUpdateAfterInput((PlayerInput)playerInput);
		playerMove.CalculateSpeed(playerInput, playerStatus, playerCollider);
		playerCollider.CheckMove (ref playerMove, ref playerStatus);
		playerStatus.statusUpdateAfterCollisionCheck (playerCollider, playerInput);
		playerMove.Move ();

        // pass variables to the animator 
        playerAnimator.SetBool("idle", playerStatus.IsIdle());
        playerAnimator.SetBool("walk", playerStatus.IsWalk());
        playerAnimator.SetBool("jump", playerStatus.IsJump());
        playerAnimator.SetBool("fall", playerStatus.IsFall() || playerStatus.IsFallCloud());
        playerAnimator.SetBool("attack", playerStatus.IsAttack());
        playerAnimator.SetBool("defense", playerStatus.IsDefense());
        playerAnimator.SetBool("refill", playerStatus.IsRefill());
        playerAnimator.SetBool("drink", playerStatus.IsDrink());
        playerAnimator.SetBool("climb", playerStatus.IsClimb());
        playerAnimator.SetBool("hit", playerStatus.IsHit());
        playerAnimator.SetBool("cast", playerStatus.IsCast());
        playerAnimator.SetBool("dead", !playerStatus.IsAlive());
        playerAnimator.SetBool("dash", playerStatus.IsDash());
        
	}

    public void switchAIInput(bool power)
    {
        if (useInputAI == power)
            return;
        useInputAI = power;
        if (useInputAI)
        {
            playerInput = GetComponent<PlayerInputAI>();
        }
        else
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }








}
