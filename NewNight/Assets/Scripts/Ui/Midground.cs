using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Midground : UiItem
	{
		[SerializeField] private float _approachSpeed=1;
		[SerializeField] private float _closeRange;

		void Update()
		{
			transform.position += Vector3.back * _approachSpeed*Time.deltaTime;
			if (transform.position.z - Camera.main.transform.position.z <_closeRange)
			{
				Destroy(gameObject);
			}
		}

	}
}
