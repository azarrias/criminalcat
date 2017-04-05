using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerInput))]
[RequireComponent (typeof(PlayerStatus))]
[RequireComponent (typeof(PlayerMove))]
[RequireComponent (typeof(PlayerCollider))]

public class PlayerController : MonoBehaviour {

	PlayerInput 	 playerInput;
	PlayerStatus     playerStatus;
	PlayerMove  	 playerMove;
	PlayerCollider   playerCollider;
	Animator		 playerAnimator;

	void Start() {
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
		
		playerStatus.statusUpdateAfterInput(playerInput);
		playerMove.CalculateSpeed(playerInput, playerStatus);
		playerCollider.CheckMove (ref playerMove, ref playerStatus);
		playerStatus.statusUpdateAfterCollisionCheck (playerCollider);
		playerMove.Move ();

        // send the animator the variables needed
/*        playerAnimator.SetBool("grounded",playerStatus.IsGrounded());
        playerAnimator.SetFloat("horizontalSpeed",Mathf.Abs(playerMove.speed.x));
        playerAnimator.SetFloat("verticalSpeed", playerMove.speed.y);
        */

        playerAnimator.SetBool("idle", playerStatus.IsIdle());
        playerAnimator.SetBool("walk", playerStatus.IsWalk());
        playerAnimator.SetBool("jump", playerStatus.IsJump());
        playerAnimator.SetBool("fall", playerStatus.IsFall());

        playerAnimator.SetBool("attack", playerStatus.IsAttack());
        playerAnimator.SetBool("defense", playerStatus.IsDefense());
        

	}








}
