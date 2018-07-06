using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLightIntensity : MonoBehaviour {


	public Light tLight;
	public float intensityMin = 0f;
	public float intensityMax = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		this.tLight.intensity = Random.Range (this.intensityMin, this.intensityMax);

	}
}
