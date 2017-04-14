using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LineOfSight))]
public class LineOfSightEditor : Editor
{
    private void OnSceneGUI()
    {
        LineOfSight fov = (LineOfSight)target;
        Handles.color = Color.white;
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);
    }
}
