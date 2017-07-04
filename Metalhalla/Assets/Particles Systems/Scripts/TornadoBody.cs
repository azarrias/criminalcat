using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoBody : MonoBehaviour {

    public int frequency = 4;
    public float resolution = 50.0f;
    public float scaleXZ = 80.0f;
    private ParticleSystem ps;

	// Use this for initialization
	void Start () {
        CreateHelix();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateHelix()
    {
        ps = GetComponent<ParticleSystem>();
        ParticleSystem.VelocityOverLifetimeModule vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.Local;
        
        AnimationCurve curveX = new AnimationCurve();
        for(int i = 0; i < resolution; i++)
        {           
            float newTime = (i / (resolution - 1));
            float value = 1 / (resolution - 1) * i * Mathf.Cos(newTime * 2 * Mathf.PI * frequency);           
            curveX.AddKey(newTime, value);
        }
        vel.x = new ParticleSystem.MinMaxCurve(scaleXZ, curveX);

        AnimationCurve curveZ = new AnimationCurve();
        for (int i = 0; i < resolution; i++)
        {            
            float newTime = (i / (resolution - 1));
            float value = 1 / (resolution - 1) * i * Mathf.Sin(newTime * 2 * Mathf.PI * frequency);            
            curveZ.AddKey(newTime, value);
        }
        vel.z = new ParticleSystem.MinMaxCurve(scaleXZ, curveZ);
    }
}
