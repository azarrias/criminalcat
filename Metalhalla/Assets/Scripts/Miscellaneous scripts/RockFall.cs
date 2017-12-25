using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RockFall : MonoBehaviour {

    public GameObject spawnPoint1 = null;
    public GameObject spawnPoint2 = null;
    public GameObject spawnPoint3 = null;
    public GameObject rockPrefab = null;
    public float spawnDelay = 2.0f;
    private bool wait = false;
    [HideInInspector]
    public bool generateRocks = false;
    private int num = 1;
    public int maxRocks = 10;
    private int rockIndex = 0;
    public GameObject[] rocks;

    void Awake () {

        rocks = new GameObject[maxRocks];

        for (int i = 0; i < maxRocks; i++)
        {
            GameObject rock = Instantiate(rockPrefab, transform.position, Quaternion.identity);
            int randomRotation = Random.Range(1, 10) * 10;
            rock.transform.Find("rock").localRotation *= Quaternion.Euler(0.0f, randomRotation, 0.0f); 
            rock.transform.parent = gameObject.transform;
            rock.SetActive(false);          
            rocks[i] = rock;
        }
	}

    // Update is called once per frame
    void Update()
    {
        if (!wait && generateRocks)
        {
            SelectSpawnPoint();        
            StartCoroutine("Delay", spawnDelay);
        }
    }

    private void SelectSpawnPoint()
    {      
        switch(num)
        {
            case 1:
                SpawnRock(spawnPoint1);
                break;

            case 2:
                SpawnRock(spawnPoint2);
                break;

            case 3:
                SpawnRock(spawnPoint3);
                break;
        }

        num++;
        if (num == 4)
            num = 1;
    }

    private void SpawnRock(GameObject sp)
    {
        wait = true;
        GameObject rock = rocks[rockIndex];
        rock.transform.position = sp.transform.position;
        rock.transform.localScale = Vector3.one;
        float randomScale = Random.Range(1.0f, 2.5f);
        rock.transform.localScale *= randomScale;
        rock.GetComponent<Rigidbody>().velocity = Vector3.zero;
        rock.transform.Find("rock_collider").GetComponent<CapsuleCollider>().enabled = true;
        rock.SetActive(true);       

        rockIndex++;
        if (rockIndex == maxRocks)
            rockIndex = 0;
    }

    private IEnumerator Delay(float seconds)
    { 
        yield return new WaitForSeconds(seconds);
        wait = false;
    }
}
