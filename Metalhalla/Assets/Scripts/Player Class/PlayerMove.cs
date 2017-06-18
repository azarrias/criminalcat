using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [Header("Horizontal move & acceleration Setup")]
    [Tooltip("Speed at which the player moves when running")]
    public float moveSpeed = 8f;
    [Tooltip("Speed at which the player moves when dashing")]
    public float dashSpeed = 15f;
    [Tooltip("Maximum time in which the player horizonal speed switches to its target value")]
    public float xSpeedChangeSpeed = 0.1f; // 0.99f previous value, too low accel

    [Header("Vertical move, jump & gravity Setup")]
    public float gravity = 40f;
    public float jumpSpeed = 5.8f;
    public float timeToJumpApex = 0.2f;
    [Tooltip("Time in which the player can fall through cloud platforms")]
    public float timeToFallThroughCloudPlatforms = 0.1f;
    [Tooltip("Climb Speed")]
    public float climbSpeed = 4f; 

    [Header("Non interactive move Setup")]
    [Tooltip("Recoil suffered when hit")]
    public float hitRecoil = 5f;

    [HideInInspector]
    public Vector3 speed;
    [HideInInspector]
    public float xCurrentSpeed;
    [HideInInspector]
    public float yCurrentSpeed;

    private float[] jumpSpeeds;

    private void Start()
    {
        CalculateJumpFramesSpeed();
    }

    public void CalculateSpeed(PlayerInput input, PlayerStatus status, PlayerCollider collider)
    {
        // horizontal speed calculations
        /*
        if (status.justHit == true)
            speed.x = status.facingRight ? -hitRecoil * Time.fixedDeltaTime : hitRecoil * Time.fixedDeltaTime;
        else if (status.IsDash() == true)
            speed.x = status.facingRight ? dashSpeed * Time.fixedDeltaTime : -dashSpeed * Time.fixedDeltaTime;
        else if (AllowHorizontalInput(status, collider) == true)
            speed.x = input.newInput.GetHorizontalInput() * moveSpeed * Time.fixedDeltaTime;
        else
            speed.x = 0;
 */
        if (status.justHit == true)
            speed.x = Mathf.SmoothDamp(speed.x, status.facingRight ? -hitRecoil * Time.fixedDeltaTime : hitRecoil * Time.fixedDeltaTime, ref xCurrentSpeed, xSpeedChangeSpeed);
        else if (status.IsDash() == true)
            speed.x = Mathf.SmoothDamp(speed.x, status.facingRight ? dashSpeed * Time.fixedDeltaTime : -dashSpeed * Time.fixedDeltaTime, ref xCurrentSpeed, xSpeedChangeSpeed);
        else if (AllowHorizontalInput(status, collider))
            speed.x = Mathf.SmoothDamp(speed.x, input.newInput.GetHorizontalInput() * moveSpeed * Time.fixedDeltaTime, ref xCurrentSpeed, xSpeedChangeSpeed);
        else 
            speed.x = Mathf.SmoothDamp(speed.x, 0, ref xCurrentSpeed, xSpeedChangeSpeed);

        // vertical speed calculations
        if (status.IsJump())
        {
            speed.y = jumpSpeeds[status.jumpFrames];
            //speed.y += (jumpSpeed - gravity * Time.fixedDeltaTime) * Time.fixedDeltaTime;
        }
        else if (status.IsFall() || status.IsFallCloud() || status.IsHit() || status.IsAttack())
        {
            speed.y += -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        }
        else if (status.IsClimb())
        {
            //speed.y = moveSpeed * Time.fixedDeltaTime * input.newInput.GetVerticalInput();
            speed.y = climbSpeed * Time.fixedDeltaTime * input.newInput.GetVerticalInput();
        }
        else
            speed.y += -gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;

    }

    public void Move()
    {
        transform.Translate(speed);
    }

    public bool AllowHorizontalInput(PlayerStatus status, PlayerCollider collider)
    {
        if (status.IsDead())
            return false;
        if (collider.IsGrounded() == false && status.WasDead() == false)
            return true;
        if (status.IsIdle() || status.IsWalk())
            return true;
        return false;
    }

    int CalculateFramesFromTime(float time)
    {
        return (int)(time / Time.fixedDeltaTime);
    }

    void CalculateJumpFramesSpeed()
    {
        int maxFrame = CalculateFramesFromTime(timeToJumpApex);
        //float initialJumpSpeed = gravity * timeToJumpApex; 
        float initialJumpSpeed = jumpSpeed;

        jumpSpeeds = new float[maxFrame + 1];

        jumpSpeeds[0] = 0;
        for (int i = 1; i < maxFrame + 1; i++)
        {
            jumpSpeeds[i] = initialJumpSpeed * i * Time.fixedDeltaTime - 0.5f * gravity * i * i * Time.fixedDeltaTime * Time.fixedDeltaTime;
            jumpSpeeds[i] -= jumpSpeeds[i - 1];
        }
    }
}
