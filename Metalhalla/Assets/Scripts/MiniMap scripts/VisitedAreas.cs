using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitedAreas : MonoBehaviour {

    private GameObject player;
    private List<GameObject> tiles = new List<GameObject>();
    private Color redColor = new Color(255, 0, 0);

    void Start () {
        foreach (Transform t in this.gameObject.transform)
        {
            if (t.gameObject.layer == LayerMask.NameToLayer("map"))
            {
                tiles.Add(t.gameObject);
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
        foreach(GameObject go in tiles)
        {
            if (player.transform.position.x > go.transform.position.x - go.transform.lossyScale.x / 2 &&
                player.transform.position.x < go.transform.position.x + go.transform.lossyScale.x / 2 &&
                player.transform.position.y > go.transform.position.y - go.transform.lossyScale.y / 2 &&
                player.transform.position.y < go.transform.position.y + go.transform.lossyScale.y / 2)
            {
                go.GetComponent<Renderer>().material.color = redColor;
            }
        }
	}
}
