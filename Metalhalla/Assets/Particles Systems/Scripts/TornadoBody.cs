using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoBody : MonoBehaviour {

    public int frequency = 4;
    public float resolution = 80;
    public float amplitude = 1.0f;
    public float zValue = 1.0f;

    private ParticleSystem ps;

	// Use this for initialization
	void Start () {
        CreateCircle();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateCircle()
    {
        ps = GetComponent<ParticleSystem>();
        ParticleSystem.VelocityOverLifetimeModule vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.Local;
        ParticleSystem.MainModule main = ps.main;
        main.startSpeed = 0f;

        
        vel.z = new ParticleSystem.MinMaxCurve(10.0f, zValue);

        AnimationCurve curveX = new AnimationCurve();
        for(int i = 0; i < resolution; i++)
        {           
            float newTime = (i / (resolution - 1));
            float value = amplitude * 1 / (resolution - 1) * i * Mathf.Sin(newTime * 2 * Mathf.PI * frequency);           
            curveX.AddKey(newTime, value);
        }
        vel.x = new ParticleSystem.MinMaxCurve(10.0f, curveX);

        AnimationCurve curveY = new AnimationCurve();
        for (int i = 0; i < resolution; i++)
        {            
            float newTime = (i / (resolution - 1));
            float value = amplitude * 1 / (resolution - 1) * i * Mathf.Cos(newTime * 2 * Mathf.PI * frequency);            
            curveY.AddKey(newTime, value);
        }
        vel.y = new ParticleSystem.MinMaxCurve(10.0f, curveY);
    }
}
