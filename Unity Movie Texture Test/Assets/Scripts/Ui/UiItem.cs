using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class UiItem : MonoBehaviour
	{

		private const float Deviation = 0.1f;
		private float _speed=1f;
		private Vector3 _destination;

		private Coroutine _runningCoroutine=null; // this method can only run one animation at the same time: no addition


		public void Transfer(Vector3 newPosition)
		{
			_destination = newPosition;
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			_runningCoroutine = StartCoroutine(PlainLerp());
		}

		private IEnumerator PlainLerp()
		{
			yield return null;
			while (Vector3.Distance(transform.position, _destination) > Deviation)
			{
				transform.position = Vector3.Lerp(transform.position, _destination,Time.deltaTime*_speed);
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
