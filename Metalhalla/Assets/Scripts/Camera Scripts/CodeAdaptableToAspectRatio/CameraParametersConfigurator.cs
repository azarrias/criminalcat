using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraFollow))]
public class CameraParametersConfigurator : MonoBehaviour
{
    //-- Manual adjustment
    public float frameSkin = 2.0f; 

    //-- Desired frame variables
    Vector3 frameCenter;
    Vector3 frameExtents;
    Vector3 frameLeft, frameRight, frameTop, frameBottom;

    //-- Camera handling variables
    Camera cameraComponent;
    Transform cameraTransform;
    float frustumHeight;
    float frustumWidth;
    float cameraAspect;

    //-- Player parameters
    float intentededPlayerToScreenHeightRatio;
    float playerAccountedHeight;

    //--Final parameters for the cameras
    Vector2 horizontalLimits;
    Vector2 verticalLimits;
    float zpos;

    public void ConfigureCamera( Vector3 newFrameCenter, Vector3 newFrameExtents)
    {
        frameCenter = newFrameCenter;
        frameExtents = newFrameExtents;

        frameLeft = frameCenter - Vector3.right * frameExtents.x;
        frameRight = frameCenter + Vector3.right * frameExtents.x;
        frameTop = frameCenter + Vector3.up * frameExtents.y;
        frameBottom = frameCenter - Vector3.up * frameExtents.y;

        /*
        frameLeft = frameCenter - Vector3.right * frameExtents.x;
        frameRight = frameCenter + Vector3.right * frameExtents.x;
        frameTop = frameCenter + Vector3.up * frameExtents.y;
        frameBottom = frameCenter - Vector3.up * frameExtents.y;
        */
        /* frameLeft = frameCenter - Vector3.right * frameExtents.x + Vector3.forward * frameExtents.z;
         frameRight = frameCenter + Vector3.right * frameExtents.x + Vector3.forward * frameExtents.z;
         frameTop = frameCenter + Vector3.up * frameExtents.y + Vector3.forward* frameExtents.z;
         frameBottom = frameCenter - Vector3.up * frameExtents.y + Vector3.forward * frameExtents.z;
         */
        intentededPlayerToScreenHeightRatio = GetComponent<CameraFollow>().playerToScreenHeightRatio;
        playerAccountedHeight = GetComponent<CameraFollow>().playerHeight;

        cameraComponent = GetComponent<Camera>();

        cameraTransform = GetComponent<Transform>();
        cameraAspect = GetComponent<Camera>().aspect;

        Vector3 lastCameraPosition = cameraTransform.position;
        CalculateCameraParameters();
        cameraTransform.position = lastCameraPosition;

        GetComponent<CameraFollow>().SetCameraParameters(horizontalLimits, verticalLimits, zpos);
        
    }

    void CalculateCameraParameters()
    {
        //1. Default distance calculation, an frustum proportions
        frustumHeight = playerAccountedHeight * 100 / intentededPlayerToScreenHeightRatio;
        zpos = -1 * frustumHeight * 0.5f / Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustumWidth = frustumHeight * cameraAspect;

        //2. Camera perpendicular of the center of the frame
        Vector3 tmp = cameraTransform.position;
        tmp.x = frameCenter.x;
        tmp.y = frameCenter.y;
        tmp.z = zpos;
        cameraTransform.position = tmp;

        //2.5 Set the vertical and horizontal limits to the center of the frame
        horizontalLimits = new Vector2(frameCenter.x, frameCenter.x);
        verticalLimits = new Vector2(frameCenter.y, frameCenter.y);

        //3. WorldPoint to ViewPort
        float frameLeftViewport = cameraComponent.WorldToViewportPoint(frameLeft).x;
        float frameTopViewport = cameraComponent.WorldToViewportPoint(frameTop).y;

        // decision based on viewport locations
        if ((frameLeftViewport >= 1.0f || frameLeftViewport <= 0.0f) && (frameTopViewport <= 0.0f || frameTopViewport >= 1.0f))
        {
            // big frame, expand limits
            horizontalLimits.x = frameLeft.x + (frameCenter.x - cameraComponent.ViewportToWorldPoint(new Vector3(0, 0, -zpos)).x) - frameSkin;
            horizontalLimits.y = frameRight.x - (cameraComponent.ViewportToWorldPoint(new Vector3(1, 0, -zpos)).x - frameCenter.x) + frameSkin;
            verticalLimits.x = frameBottom.y + (frameCenter.y - cameraComponent.ViewportToWorldPoint(new Vector3(0, 0, -zpos)).y) - frameSkin;
            verticalLimits.y = frameTop.y - (cameraComponent.ViewportToWorldPoint(new Vector3(0, 1, -zpos)).y - frameCenter.y) + frameSkin;
        }
        else if ((frameLeftViewport >= 0.0f || frameLeftViewport <= 0.0f) && (frameTopViewport < 0.0f || frameTopViewport > 1.0f))
        {
            // zoom in - horizonal extents set the required zoom
            frustumWidth = (frameExtents.x + frameSkin)* 2;
            frustumHeight = frustumWidth / cameraAspect;
            zpos = -1 * frustumHeight * 0.5f / Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
            // set vertical limits with recalculated zoom
            verticalLimits.x = frameBottom.y + (frameCenter.y - cameraComponent.ViewportToWorldPoint(new Vector3(0, 0, -zpos)).y) - frameSkin;
            verticalLimits.y = frameTop.y - (cameraComponent.ViewportToWorldPoint(new Vector3(0, 1, -zpos)).y - frameCenter.y) + frameSkin;
        }
        else if ((frameLeftViewport > 1.0f || frameLeftViewport < 0.0f) && (frameTopViewport >= 0.0f || frameTopViewport <= 1.0f))
        {
            // zoom in - vertical extents set the required zoom
            frustumHeight = (frameExtents.y + frameSkin)* 2;
            zpos = -1 * frustumHeight * 0.5f / Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
            frustumWidth = frustumHeight * cameraAspect;
            // set horizontal limits with recalculated zoom
            horizontalLimits.x = frameLeft.x + (frameCenter.x - cameraComponent.ViewportToWorldPoint(new Vector3(0, 0, -zpos)).x) - frameSkin;
            horizontalLimits.y = frameRight.x - (cameraComponent.ViewportToWorldPoint(new Vector3(1, 0, -zpos)).x - frameCenter.x) + frameSkin;
        }
        else
        {
            // get the frame ratio to know which axis to use zoom
            float frameAspectRatio = frameExtents.x / frameExtents.y;
            if (frameAspectRatio >= cameraAspect)
            {
                // zoom in - horizonal extents set the required zoom
                frustumWidth = (frameExtents.x + frameSkin) * 2;
                frustumHeight = frustumWidth / cameraAspect;
                zpos = -1 * frustumHeight * 0.5f / Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
                // set vertical limits with recalculated zoom
                verticalLimits.x = frameBottom.y + (frameCenter.y - cameraComponent.ViewportToWorldPoint(new Vector3(0, 0, -zpos)).y) - frameSkin;
                verticalLimits.y = frameTop.y - (cameraComponent.ViewportToWorldPoint(new Vector3(0, 1, -zpos)).y - frameCenter.y) + frameSkin;
            }
            else
            {
                // zoom in - vertical extents set the required zoom
                frustumHeight = (frameExtents.y + frameSkin) * 2;
                zpos = -1 * frustumHeight * 0.5f / Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
                frustumWidth = frustumHeight * cameraAspect;
                // set horizontal limits with recalculated zoom
                horizontalLimits.x = frameLeft.x + (frameCenter.x - cameraComponent.ViewportToWorldPoint(new Vector3(0, 0, -zpos)).x) - frameSkin;
                horizontalLimits.y = frameRight.x - (cameraComponent.ViewportToWorldPoint(new Vector3(1, 0, -zpos)).x - frameCenter.x) + frameSkin;
            }
        }

    }


}
