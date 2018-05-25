using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class UiItem : MonoBehaviour
	{

		private const float Deviation = 0.1f;
		private float _speed=1f; // later make this adjustable
		private Vector3 _originalPosition;
		
		
		private Vector3 _destination;
		private bool _isInScreenSpace;
		

		private Coroutine _runningCoroutine=null; // this method can only run one animation at the same time: no addition


		public Vector3 GetOriginalPosition()
		{
			return _originalPosition;
		}

		public void Transfer(Vector3 newPosition, bool inScreenSpace, bool replaceOrigin)
		{
			_destination = newPosition;
			_isInScreenSpace = inScreenSpace;
			if (replaceOrigin)
			{
				if (inScreenSpace)
				{
					Debug.LogWarning("Unsafe usage: transfer an UiItem with replaceOrigin==true while inScreenSpace==true");
					_originalPosition = Coordinate.instance.Screen2Space(newPosition);
				}
				else _originalPosition = newPosition;
			}
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			_runningCoroutine = StartCoroutine(PlainLerp());
		}

		private IEnumerator PlainLerp()
		{
			yield return null;
			while (true)
			{
				transform.position = Vector3.Lerp(transform.position, (_isInScreenSpace?Coordinate.instance.Screen2Space(_destination):_destination),Time.deltaTime*_speed);
				yield return new WaitForEndOfFrame();
				if (Vector3.Distance(transform.position, (_isInScreenSpace?Coordinate.instance.Screen2Space(_destination):_destination)) < Deviation) break;
			}
		}
	}
}
