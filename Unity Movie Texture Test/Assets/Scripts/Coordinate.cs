using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate : MonoBehaviour
{

	public static Coordinate instance;

	// Use this for initialization
	void Awake()
	{
		if (instance == null) instance = this;
		else
		{
			Debug.Log("Coordinate instance exists when awake");
		}
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Vector3 Space2Screen(Vector3 Cor_Space)
	{
		float tanFV = Mathf.Tan(Camera.main.fieldOfView*Mathf.PI/180);
		return new Vector3(Cor_Space.x*(Screen.height/Screen.width)/(tanFV*Cor_Space.z),
			Cor_Space.y/(Cor_Space.z*tanFV),Cor_Space.z);
	}

	public Vector3 Screen2Space(Vector3 Cor_Screen)
	{
		float tanFV = Mathf.Tan(Camera.main.fieldOfView*Mathf.PI/180);
		return new Vector3(Cor_Screen.x*Cor_Screen.z*tanFV*(Screen.height/Screen.width),
			Cor_Screen.y*Cor_Screen.z*tanFV,Cor_Screen.z);
	}
}
