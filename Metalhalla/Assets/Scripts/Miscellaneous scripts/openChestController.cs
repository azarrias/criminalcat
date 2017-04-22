using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openChestController : MonoBehaviour
{

    public GameObject content;
    public Vector3 contentPositionOffset = new Vector3(0, 1, 0);

    enum state { CLOSED, OPEN };
    state currentState = state.CLOSED;

    private void Start()
    {
        currentState = state.CLOSED;
    }

    // when attacking the chest it gets open
    public void ApplyDamage(int dmg = 0)
    {
        if (currentState == state.CLOSED)
        {
            currentState = state.OPEN;
            // start the open animation
            /**/
            // instance the contents
            if (content != null)
                Instantiate(content, contentPositionOffset, content.transform.rotation);
        }

    }
}
