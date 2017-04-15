using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
public class FSMEnemy : MonoBehaviour {

    [HideInInspector]
    public LineOfSight los;

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
    }
}
