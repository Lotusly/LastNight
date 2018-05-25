using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Ui
{
	public class MarkCircle : MonoBehaviour
	{

		private const float RotatingSpeed = 60;
		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			transform.Rotate(Vector3.forward*RotatingSpeed*Time.deltaTime);
		}
	}
}
