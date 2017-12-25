using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(BoxCollider2D))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerCollider : MonoBehaviour {

	public LayerMask noCloudCollisionMask;
	public LayerMask generalCollisionMask;
    public LayerMask onlyCloudCollisionMask;

	const float skinWidth = .05f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	public float maxSlopeClimbAngle = 45;

	float horizontalRaySpacing;
	float verticalRaySpacing;

    //BoxCollider2D collider;
    BoxCollider collider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	void Start(){
        collider = GetComponent<BoxCollider>();
		CalculateRaySpacing ();
	}

	public void CheckMove( ref PlayerMove playerMove, ref PlayerStatus status ){
		UpdateRaycastOrigins ();
		collisions.Reset ();
		if (playerMove.speed.x != 0)
			HorizontalCollisions (ref playerMove.speed);
		if (playerMove.speed.y != 0)
			VerticalCollisions (ref playerMove.speed, ref status);
	}
		
	void HorizontalCollisions(ref Vector3 speed){
		float directionX = Mathf.Sign (speed.x);
		float rayLength = Mathf.Abs (speed.x) + skinWidth;
		for (int i = 0; i < horizontalRayCount; i++) {

			/*Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            */
            Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector3.up * (horizontalRaySpacing * i);

            RaycastHit hit;

			Debug.DrawRay (rayOrigin, Vector3.right*directionX*rayLength , Color.red);

            if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, generalCollisionMask))
            { 
            // this is ok since we check the bottom most with a slope
            float surfaceAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (i == 0 && surfaceAngle <= maxSlopeClimbAngle) {
					float distanceToSlopeStart = 0;
					if (surfaceAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						speed.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref speed, surfaceAngle);
					speed.x += distanceToSlopeStart * directionX;
				}

				if ((!collisions.climbingSlope || surfaceAngle > maxSlopeClimbAngle) && (hit.transform.gameObject.layer != noCloudCollisionMask)) {
					speed.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						speed.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (speed.x);
					}
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}

			}
		}
	}

	void VerticalCollisions(ref Vector3 speed, ref PlayerStatus status ){
		float directionY = Mathf.Sign (speed.y);
		float rayLength = Mathf.Abs (speed.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {

            /*
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + speed.x);
            
            RaycastHit hit;
            bool has_hit = false;

            if (directionY < 0 && status.IsFallCloud() == false )
                has_hit = Physics.Raycast(rayOrigin, Vector2.up * directionY, out hit, rayLength, generalCollisionMask);
            else
                has_hit = Physics.Raycast(rayOrigin, Vector2.up * directionY, out hit, rayLength, noCloudCollisionMask);

            Debug.DrawRay (rayOrigin, Vector2.up*directionY *rayLength , Color.red);
            */
            Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector3.right * (verticalRaySpacing * i + speed.x);

            RaycastHit hit;
            bool has_hit = false;

            if (directionY < 0 && status.IsFallCloud() == false)
                has_hit = Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, generalCollisionMask);
            else
                has_hit = Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, noCloudCollisionMask);

            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);

            if (has_hit )
            { 
				speed.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					speed.x = speed.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (speed.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

			}
		}
	}

	void ClimbSlope (ref Vector3 speed, float slopeAngle){
		float moveDistance = Mathf.Abs (speed.x);
		float climbSpeedY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
		if (speed.y <= climbSpeedY) {
			speed.y = climbSpeedY;
			speed.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (speed.x);
			collisions.below = true;
			collisions.climbingSlope = true;
		}
	}



	void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);
        /*
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
        */
        raycastOrigins.bottomLeft = new Vector3(bounds.min.x, bounds.min.y, bounds.center.z);
        raycastOrigins.bottomRight = new Vector3(bounds.max.x, bounds.min.y, bounds.center.z);
        raycastOrigins.topLeft = new Vector3(bounds.min.x, bounds.max.y, bounds.center.z);
        raycastOrigins.topRight = new Vector3(bounds.max.x, bounds.max.y, bounds.center.z);
    }

	void CalculateRaySpacing(){
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

    public bool PlayerAboveCloudPlatform()
    {
        for (int i = 0; i < verticalRayCount; i++)
        {/*
            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit hitinfo;
            if (Physics.Raycast(rayOrigin, -Vector2.up, out hitinfo, skinWidth*2, onlyCloudCollisionMask))
                return true;
                */
            Vector3 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector3.right * (verticalRaySpacing * i);
            RaycastHit hitinfo;
            if (Physics.Raycast(rayOrigin, -Vector3.up, out hitinfo, skinWidth * 2, onlyCloudCollisionMask))
                return true;
        }
        return false;
    }

    public bool IsGrounded()
    {
        return collisions.below == true;
    }

    struct RaycastOrigins {
        /*
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
        */
        public Vector3 topLeft, topRight;
        public Vector3 bottomLeft, bottomRight;
    }

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public float slopeAngle, slopeAngleOld;

		public void Reset(){
			above = below = left = right = false;
			climbingSlope = false;
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}
