using UnityEngine;

[RequireComponent(typeof (CharacterMovement))]
public class UserControl : MonoBehaviour
{

    private CharacterMovement character;
    private bool jump;
   
	void Awake ()
    {
        character = GetComponent<CharacterMovement>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!jump)
            jump = Input.GetButtonDown("Jump");
	}

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        character.Move(h, 0, jump);   
        jump = false;
    }
}
