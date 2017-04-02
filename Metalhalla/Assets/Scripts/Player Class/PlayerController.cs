using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerInput))]
[RequireComponent (typeof(PlayerStatus))]
[RequireComponent (typeof(PlayerMove))]
[RequireComponent (typeof(PlayerCollider))]
[RequireComponent (typeof(PlayerHealth))]

public class PlayerController : MonoBehaviour {

	PlayerInput 	 playerInput;
	PlayerStatus     playerStatus;
	PlayerMove  	 playerMove;
	PlayerCollider   playerCollider;
    PlayerHealth     playerHealth;
	Animator		 playerAnimator;

	void Start() {
		playerInput = GetComponent<PlayerInput> ();
		playerStatus = GetComponent<PlayerStatus> ();
		playerMove = GetComponent<PlayerMove> ();
		playerCollider = GetComponent<PlayerCollider> ();
        playerHealth = GetComponent<PlayerHealth> (); 
	//	playerAnimator = GetComponent<Animator> ();
	}

	void Update () {
		playerInput.GetInput ();
    }
	void FixedUpdate (){
		
		playerStatus.statusUpdateAfterInput(playerInput);
		playerMove.CalculateSpeed(playerInput, playerStatus);
		playerCollider.CheckMove (ref playerMove, ref playerStatus);
		playerStatus.statusUpdateAfterCollisionCheck (playerCollider);
		playerMove.Move ();

		// send the animator the variables needed
	//	playerAnimator.SetBool("inAir",!playerStatus.newStatus.IsGround());
	//	playerAnimator.SetFloat("speed",Mathf.Abs(playerMove.speed.x));

	}








}
