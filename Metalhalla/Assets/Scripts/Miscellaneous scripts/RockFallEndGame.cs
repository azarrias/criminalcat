using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFallEndGame : MonoBehaviour {

    public List<GameObject> spawnPoints;
    public GameObject rockPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartRockFall()
    {
        rockPrefab.GetComponent<RockBehaviour>().damage = 1000;

        foreach(GameObject sp in spawnPoints)
        {
            GameObject rock = Instantiate(rockPrefab, sp.transform.position, Quaternion.identity);
            rock.transform.localScale *= Random.Range(3, 6);
            rock.transform.localRotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 180.0f), 0.0f);          
        }
    }
}
