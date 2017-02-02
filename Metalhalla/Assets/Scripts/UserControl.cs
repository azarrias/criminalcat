using UnityEngine;

[RequireComponent(typeof (Character))]
public class UserControl : MonoBehaviour
{

    private Character character;
    private bool jump;
   
	void Awake ()
    {
        character = GetComponent<Character>();
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
