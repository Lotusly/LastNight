using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Ui
{
	public class MarkCircle : MonoBehaviour
	{

		private Animator _anim;

		private const float RotatingSpeed = 60;
		// Use this for initialization
		void Start()
		{
			_anim = GetComponent<Animator>();
		}

		// Update is called once per frame
		void Update()
		{
			transform.Rotate(Vector3.forward*RotatingSpeed*Time.deltaTime);
		}

		public void Pop()
		{
			_anim.SetTrigger("POP");
			_anim.ResetTrigger("SHRINK");
		}

		public void Shrink()
		{
			_anim.SetTrigger("SHRINK");
			_anim.ResetTrigger("POP");
		}
	}
}
