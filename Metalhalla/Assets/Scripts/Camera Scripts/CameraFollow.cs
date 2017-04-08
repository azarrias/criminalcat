using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Header("Basic Setup")]
    [Tooltip("Player instance to follow")]
    public GameObject player;
    [Tooltip("How tall is the player?")]
    public float playerHeight = 2.0f;
    [Tooltip("Player to Screen Ratio (in %). 100% means the player appears as tall as the screen. Calculations used using PERSPECTIVE projection")]
    [Range(1f, 100f)]
    public float playerToScreenHeightRatio = 15;

    [Header("Move Zone Limits")]
    [Tooltip("Zone where the camera follows the player. If the players leaves the zone, the camera snaps to the limits specified.")]
    public float limitLeft;
    public float limitRight;
    public float limitTop;
    public float limitBottom;

    [Header("Move Zone Debug")]
    [Tooltip("Draws a box with the move zone, and the center position of the camera")]
    public bool showMoveBox = true;

    private Vector3 playerPosition;
    private float distanceFromPlayer = -10;

    bool activeTracking = true;
    [Range(0, 1)]
    public float transitionSpeed = 0.05f;

    Vector3 lastCameraPositionBeforeActiveTracking;
    Vector3 lastTargetPosition;
    float transitionThreshold;


    void Start()
    {
        if (player == null)
        {
            Debug.LogError("There is no target to follow attached to the script");
            player = this.gameObject;
        }
        else
        {
            playerPosition = player.transform.position;
        }
        SetLimits(limitLeft, limitRight, limitTop, limitBottom);
        lastCameraPositionBeforeActiveTracking = transform.position;
    }


    void LateUpdate()
    {
        SetCameraPosition();
        if (showMoveBox)
            ShowBox();
 //       Debug.Log("activetracking: " + activeTracking);
    }

    void SetCameraPosition()
    {
        Vector3 playerPositionDiff = player.transform.position - playerPosition;
        playerPosition = player.transform.position;

        if (activeTracking == true)
        {
            Vector3 cameraPosition = transform.position;

            if (playerPosition.x > limitLeft && playerPosition.x < limitRight)
                cameraPosition.x = playerPosition.x;
            else
            {
                if (playerPosition.x < limitLeft && Mathf.Abs(cameraPosition.x - limitLeft) > 0.1f)
                    cameraPosition.x = limitLeft;
                else if (playerPosition.x > limitRight && Mathf.Abs(cameraPosition.x - limitRight) > 0.1f)
                    cameraPosition.x = limitRight;
            }

            if (playerPosition.y < limitTop && playerPosition.y > limitBottom)
                cameraPosition.y = playerPosition.y;
            else
            {
                if (playerPosition.y > limitTop && Mathf.Abs(cameraPosition.y - limitTop) > 0.1f)
                    cameraPosition.y = limitTop;
                else if (playerPosition.y < limitBottom && Mathf.Abs(cameraPosition.y - limitBottom) > 0.1f)
                    cameraPosition.y = limitBottom;
            }

            transform.position = cameraPosition;

     //       CorrectOutOfBounds();

            lastCameraPositionBeforeActiveTracking = transform.position;
        }
        else
        {
            if (lastTargetPosition != GetTargetPosition())
            {
                lastTargetPosition = GetTargetPosition();
            }
            transitionThreshold = Vector3.Distance(lastCameraPositionBeforeActiveTracking, lastTargetPosition) /8;
            MoveCamera( lastTargetPosition, transitionSpeed);
        }


    }

    void ShowBox()
    {
        Vector3 topLeft = new Vector3(limitLeft, limitTop);
        Vector3 topRight = new Vector3(limitRight, limitTop);
        Vector3 bottomLeft = new Vector3(limitLeft, limitBottom);
        Vector3 bottomRight = new Vector3(limitRight, limitBottom);
        Vector3 centerPos = transform.position;
        centerPos.z = -1.0f;
        Color lineColor = Color.cyan;

        Debug.DrawLine(centerPos + Vector3.left * 0.2f, centerPos + Vector3.right * 0.2f, lineColor);
        Debug.DrawLine(centerPos + Vector3.up * 0.2f, centerPos + Vector3.down * 0.2f, lineColor);
        Debug.DrawLine(topLeft, topRight, lineColor);
        Debug.DrawLine(topRight, bottomRight, lineColor);
        Debug.DrawLine(bottomRight, bottomLeft, lineColor);
        Debug.DrawLine(bottomLeft, topLeft, lineColor);
    }

    public void SetLimits(float left, float right, float top, float bottom)
    {
        limitLeft = left;
        limitRight = right;
        limitTop = top;
        limitBottom = bottom;
    }

    void CorrectOutOfBounds()
    {
        Vector3 correctedPosition = transform.position;
        if (correctedPosition.x < limitLeft)
            correctedPosition.x = Mathf.Lerp(correctedPosition.x, limitLeft, 0.5f);
        else if (correctedPosition.x > limitRight)
            correctedPosition.x = Mathf.Lerp(correctedPosition.x, limitRight, 0.5f);

        if (correctedPosition.y > limitTop)
            correctedPosition.y = limitTop;
        else if (correctedPosition.y < limitBottom)
            correctedPosition.y = limitBottom;

        transform.position = correctedPosition;
    }

    void CalculateDistanceFromPlayer()
    {
        float frustumHeight = playerHeight * 100 / playerToScreenHeightRatio;
        distanceFromPlayer = -1 * frustumHeight * 0.5f / Mathf.Tan(GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    public void SetPlayerToScreenHeightRatio(float new_ratio)
    {
        playerToScreenHeightRatio = new_ratio;
        CalculateDistanceFromPlayer();
    }

    public void SetCameraParameters(Vector2 horizontalLimits, Vector2 verticalLimits, float newDistanceFromPlayer)
    {
        if (horizontalLimits.x != limitLeft || horizontalLimits.y != limitRight || verticalLimits.x != limitBottom || verticalLimits.y != limitTop || newDistanceFromPlayer != distanceFromPlayer)
        {
            activeTracking = false;
            distanceFromPlayer = newDistanceFromPlayer;
            SetLimits(horizontalLimits.x, horizontalLimits.y, verticalLimits.y, verticalLimits.x);
        }
    }

    //-- scene switch
    public void MoveCamera(Vector3 targetPos, float moveSpeed)
    {
        StartCoroutine(MoveToNextScene(targetPos, moveSpeed));
    }

    IEnumerator MoveToNextScene( Vector3 targetPos, float moveSpeed)
    {
        activeTracking = false;

        while (Vector3.Distance(transform.position, targetPos)> transitionThreshold)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed); 
            yield return 0;
        }

        activeTracking = true;
    }

    Vector3 GetTargetPosition()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 targetPosition = new Vector3();

        if (playerPosition.x < limitLeft)
            targetPosition.x = limitLeft;
        else if (playerPosition.x > limitRight)
            targetPosition.x = limitRight;
        else
            targetPosition.x = playerPosition.x;

        if (playerPosition.y > limitTop)
            targetPosition.y = limitTop;
        else if (playerPosition.y < limitBottom)
            targetPosition.y = limitBottom;
        else
            targetPosition.y = playerPosition.y;

        targetPosition.z = distanceFromPlayer;
        return targetPosition;
    }
}
