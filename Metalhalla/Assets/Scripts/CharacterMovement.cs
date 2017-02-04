using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float jumpForce = 400f;
    [SerializeField]
    private LayerMask whatIsGround;

    private Transform groundCheck; // A position marking where to check if the player is grounded
    private float groundedRadius = 0.1f; // Radius of the overlap sphere to determine if grounded
    private bool grounded = false; // Whether or not the player is grounded
    private Vector3 tmp;
   
	private void Awake ()
    {
        // Setting up references
        groundCheck = transform.Find("GroundCheck");
	}


    private void FixedUpdate()
    {
        // The player is grounded if a spherecast to the groundcheck position hits anything designated as ground
        //Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundedRadius, whatIsGround);
        Vector3 halfExtents = GetComponent<Collider>().bounds.extents;
        halfExtents.x -= 0.1f;
        halfExtents.z -= 0.1f;
        Collider[] colliders = Physics.OverlapBox(transform.position, halfExtents, transform.rotation, whatIsGround);
        grounded = colliders.Length != 0 ? true : false;
    }

    public void Move(float moveHor, float moveVert, bool jump)
    {
        if(grounded)
        {
            //Move the character
            GetComponent<Rigidbody>().velocity = new Vector3(moveHor * speed, GetComponent<Rigidbody>().velocity.y, 0f);
            
        }

        // If the player should jump
        if(grounded && jump)
        {
            //Add vertical force to the player
            grounded = false;
            GetComponent<Rigidbody>().AddForce(new Vector3(0f, jumpForce, 0f));

        }
    }
        
}

