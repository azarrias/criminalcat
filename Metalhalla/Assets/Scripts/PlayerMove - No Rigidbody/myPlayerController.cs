using UnityEngine;
using System.Collections;

[RequireComponent (typeof(myPlayerInput))]
[RequireComponent (typeof(myPlayerStatus))]
[RequireComponent (typeof(myPlayerMove))]
[RequireComponent (typeof(myPlayerCollider))]


public class myPlayerController : MonoBehaviour {

	myPlayerInput 	 playerInput;
	myPlayerStatus   playerStatus;
	myPlayerMove	 playerMove;
	myPlayerCollider playerCollider;
	Animator		 playerAnimator;

	void Start() {
		playerInput = GetComponent<myPlayerInput> ();
		playerStatus = GetComponent<myPlayerStatus> ();
		playerMove = GetComponent<myPlayerMove> ();
		playerCollider = GetComponent<myPlayerCollider> ();
	//	playerAnimator = GetComponent<Animator> ();
	}

	void Update () {
		playerInput.GetInput ();
	}
	void FixedUpdate (){
		// faltará actualizar el estado
		//playerInput.GetInput ();
		playerStatus.statusUpdateAfterInput(playerInput);
		playerMove.CalculateSpeed(playerInput,playerStatus);
		playerCollider.CheckMove (ref playerMove);
		playerStatus.statusUpdateAfterCollisionCheck (playerCollider);
		playerMove.Move ();
		// send the animator what needs to be sent :)
	//	playerAnimator.SetBool("inAir",!playerStatus.newStatus.IsGround());
	//	playerAnimator.SetFloat("speed",Mathf.Abs(playerMove.speed.x));

	}








}
