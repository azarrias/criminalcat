using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [Header("Basic Setup")]
    [Tooltip("Player instance to follow")]
    public GameObject player;

    // Activate your position limitations for the Y axis by turning this on.

    [Header("Deadzone Limits Setup")]
    [Tooltip("Limits where the camera stops following the player. Must be updated when entering a new zone")]
    public float limitLeft;
    public float limitRight;
    public float limitBottom;
    public float limitTop;

    [Header("Debug Visuals")]
    [Tooltip("Draws a debug box with the dead zone of the camera")]
    public bool showDebugBox = false;

    private Vector3 cameraPosition;
    private Vector3 playerPosition;

    void Start()
    {
        cameraPosition = transform.position;

        if (player == null)
            Debug.LogError("There is no target to follow attached to the script");

    }


    void LateUpdate()
    {
        CameraUpdate();
        if (showDebugBox)
            DrawDebugBox();
    }

    void CameraUpdate()
    {
        Vector3 positionUpdate = Vector3.zero;
        Vector3 playerPositionDif = player.transform.position - playerPosition;
        playerPosition = player.transform.position;
        
        // horizontal followup
        if (playerPosition.x > limitLeft &&  playerPosition.x < limitRight)
            positionUpdate.x = playerPositionDif.x;

        // vertical followup
        if (playerPosition.y < limitTop && playerPosition.y > limitBottom)
            positionUpdate.y = playerPositionDif.y;

        if (positionUpdate.Equals(Vector3.zero) == false)
        {
            transform.position += positionUpdate;
        }
        CorrectOutOfBounds();
    }

    void DrawDebugBox()
    {
        Vector3 centerPos = transform.position;
        centerPos.z = 0;

        Vector3 topLeft = new Vector3(limitLeft, limitTop, 0);
        Vector3 bottomLeft = new Vector3(limitLeft, limitBottom, 0);
        Vector3 topRight = new Vector3(limitRight, limitTop, 0);
        Vector3 bottomRight = new Vector3(limitRight, limitBottom, 0);

        Color lineColor = Color.cyan;
        Debug.DrawLine(centerPos + Vector3.up * 0.2f, centerPos + Vector3.down * 0.2f, lineColor);
        Debug.DrawLine(centerPos + Vector3.left * 0.2f, centerPos + Vector3.right * 0.2f, lineColor);

        Debug.DrawLine(topLeft, bottomLeft, lineColor);
        Debug.DrawLine(topLeft, topRight, lineColor);
        Debug.DrawLine(bottomLeft, bottomRight, lineColor);
        Debug.DrawLine(topRight, bottomRight, lineColor);
    }

    public void SetLimits(float leftLimit, float rightLimit, float bottomLimit, float topLimit)
    {
        limitLeft = leftLimit;
        limitRight = rightLimit;
        limitBottom = bottomLimit;
        limitTop = topLimit;
    }

    void CorrectOutOfBounds()
    {
        Vector3 correctedPosition = transform.position;
        if (correctedPosition.x < limitLeft)
            correctedPosition.x = limitLeft;
        else if (correctedPosition.x > limitRight)
            correctedPosition.x = limitRight;
        if (correctedPosition.y > limitTop)
            correctedPosition.y = limitTop;
        else if (correctedPosition.y < limitBottom)
            correctedPosition.y = limitBottom;

        transform.position = correctedPosition;
    }


}
