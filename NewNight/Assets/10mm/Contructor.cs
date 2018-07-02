using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contructor : MonoBehaviour
{
	private Material _mat;
	void Awake()
	{
		_mat = GetComponent<Renderer>().material;
	}

	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator FormCoroutine(float speed)
	{
		yield return null;
		
	}
	
}
