using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RockFall : MonoBehaviour {

    public GameObject spawnPoint1 = null;
    public GameObject spawnPoint2 = null;
    public GameObject spawnPoint3 = null;
    public GameObject rockPrefab = null;
    private Transform currentSpawnPoint = null;
    public float spawnDelay = 2.0f;
    private bool wait = false;
    public bool active = false;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (!wait && active)
        {
            SelectSpawnPoint();
            StartCoroutine("Delay", spawnDelay);
        }

    }

    private void SelectSpawnPoint()
    {
        System.Random rand = new System.Random();
        int num = rand.Next(0, 3);

        switch(num)
        {
            case 0:
                SpawnRock(spawnPoint1);
                break;
            case 1:
                SpawnRock(spawnPoint2);
                break;
            case 2:
                SpawnRock(spawnPoint3);
                break;
        }

    }

    private void SpawnRock(GameObject sp)
    {
        wait = true;
        GameObject rock = Instantiate(rockPrefab, sp.transform.position, Quaternion.identity);
        rock.transform.parent = transform;
    }

    private IEnumerator Delay(float seconds)
    {
        System.Random rand = new System.Random();
        double randomTime = rand.NextDouble();
        yield return new WaitForSeconds(seconds + (float)randomTime);
        wait = false;
    }
}
