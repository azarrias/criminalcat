using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingEvent : MonoBehaviour {

    public IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }

}
