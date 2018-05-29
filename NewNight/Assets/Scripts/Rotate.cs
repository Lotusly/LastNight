using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

	[SerializeField] private float _speedRotating;

	[SerializeField] private Vector3 _rotateDirection;
	// Use this for initialization
	void Start ()
	{
		_rotateDirection = _rotateDirection.normalized;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(_rotateDirection*_speedRotating*Time.deltaTime,Space.World);
	}
}
