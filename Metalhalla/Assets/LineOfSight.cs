using System.Collections;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask obstacleMask;
    private GameObject player;

    public void Awake()
    {

    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(-Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), -Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}
