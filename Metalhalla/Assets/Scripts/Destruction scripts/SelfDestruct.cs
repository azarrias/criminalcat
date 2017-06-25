using UnityEngine;
using System.Collections.Generic;

public class SelfDestruct : MonoBehaviour
{

    public float lifeTime = 2.0f;
    private float lifeTimeCounter = 0.0f;
    public float fadeoutSpeed = 0.01f;
    private List<MeshRenderer> fragmentsMRList;


    // Use this for initialization
    void Start()
    {
        fragmentsMRList = new List<MeshRenderer>();

        if (gameObject.name == "RockFragments12(Clone)")
        {
            foreach (Transform fragmentTr in transform)
            {
                MeshRenderer mr = fragmentTr.Find("rock_mesh").gameObject.GetComponent<MeshRenderer>();
                fragmentsMRList.Add(mr);
            }
        }
        else //script attached to crate
        {
            foreach (Transform fragmentTr in transform)
            {
                if (fragmentTr.name != "SphereCollider")
                {
                    MeshRenderer mr = fragmentTr.gameObject.GetComponent<MeshRenderer>();
                    fragmentsMRList.Add(mr);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimeCounter += Time.deltaTime;

        //fadeout
        foreach (MeshRenderer mr in fragmentsMRList)
        {
            Color color = mr.materials[0].color;

            color.a -= fadeoutSpeed;
            if (color.a < 0.0f)
                color.a = 0.0f;

            mr.materials[0].color = color;
        }
        
        if(lifeTimeCounter >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}