using System.Collections;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask obstacleMask;
    private GameObject player;
    [HideInInspector]
    public bool playerInSight = false;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.LogError("No game object has the tag Player");
        }
    }

    void Start()
    {
        StartCoroutine("FindPlayer", 0.3f);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(-Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), -Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    IEnumerator FindPlayer(float dt)
    {
        while (true)
        {
            yield return new WaitForSeconds(dt);
            playerInSight = PlayerInSight();
            if (playerInSight)
            {
                Debug.Log(this.name.ToString() + ": I can see the player");
                Debug.DrawLine(player.transform.position, transform.position);
            }
        }
    }

    public bool PlayerInSight()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer < viewRadius && Vector3.Angle(-transform.right, directionToPlayer) < viewAngle/2)
        {
            // Player is visible to the GO if it is within sight distance and angle, and not covered by an obstacle
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                return true;
        }
        return false;
    }
}
