using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManagerScript : MonoBehaviour {

    public static ObjectPoolManagerScript instance;
    public GameObject pooledObject;
    public int pooledAmount = 20;

    List<GameObject> pooledObjects;

    private void Awake()
    {
        instance = this;
    }

    private void Start() {
        pooledObjects = new List<GameObject>();
        for(int i = 0; i < pooledAmount; ++i)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
	}

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledObjects.Count; ++i)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        GameObject obj = (GameObject)Instantiate(pooledObject);
        pooledObjects.Add(obj);
        return obj;
    }
	
}
