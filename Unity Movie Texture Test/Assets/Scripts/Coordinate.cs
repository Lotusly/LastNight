using System.Collections;

using UnityEngine;

public class Coordinate : Singleton<Coordinate>
{

	

	// Use this for initialization
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Vector3 Space2Screen(Vector3 Cor_Space)
	{
		float tanFV = Mathf.Tan(Camera.main.fieldOfView*Mathf.PI/360);
		return new Vector3(Cor_Space.x*((float)(Screen.height)/Screen.width)/(tanFV*Cor_Space.z),
			Cor_Space.y/(Cor_Space.z*tanFV),Cor_Space.z);
	}

	public Vector3 Screen2Space(Vector3 Cor_Screen)
	{
		float tanFV = Mathf.Tan(Camera.main.fieldOfView*Mathf.PI/360);
		return new Vector3(Cor_Screen.x*Cor_Screen.z*tanFV*((float)(Screen.width)/Screen.height),
			Cor_Screen.y*Cor_Screen.z*tanFV,Cor_Screen.z);
	}
}
