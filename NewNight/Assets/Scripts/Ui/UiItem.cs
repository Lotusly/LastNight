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
		private bool _isInScreenSpace;// used in both Follow Camera and Lerp To Transfer

		private bool _followingCamera = false; // UiItem should have function to stay static to the camera
		

		private Coroutine _runningCoroutine=null; // this method can only run one animation at the same time: no addition

		void Update()
		{
			if (_followingCamera)
			{
				if (!_isInScreenSpace)
				{
					Debug.LogError("UiItem follow camera while not in screen space");
					_followingCamera = false;
				}
				else
				{
					transform.position = Coordinate.instance.Screen2Space(_destination);
				}
			}
		}


		public Vector3 GetOriginalPosition()
		{
			return _originalPosition;
		}

		public virtual void MoveBack()
		{
			Transfer(_originalPosition,false,false);
		}

		public void EnableFollowCamera()
		{
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			_destination = _isInScreenSpace ? _destination : Coordinate.instance.Space2Screen(_destination);
			_isInScreenSpace = true;
			_followingCamera = true;
			
		}

		public void DisableFolloeCamera()
		{
			_followingCamera = false;
		}


		public void SetPosition(Vector3 newPosition, bool inScreenSpace, bool replaceOrigin, bool followCamera = false)
		{
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			_destination = newPosition;
			_isInScreenSpace = inScreenSpace;
			if (replaceOrigin) _originalPosition = newPosition;
			transform.position = (inScreenSpace ? Coordinate.instance.Screen2Space(newPosition) : newPosition);
			if(followCamera) EnableFollowCamera();
		}


		public void Transfer(Vector3 newPosition, bool inScreenSpace, bool replaceOrigin, bool followCamera = false)
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
			_runningCoroutine = StartCoroutine(PlainLerp(followCamera));
		}

		private IEnumerator PlainLerp(bool followCamera)
		{
			yield return null;
			while (true)
			{
				transform.position = Vector3.Lerp(transform.position, (_isInScreenSpace?Coordinate.instance.Screen2Space(_destination):_destination),Time.deltaTime*_speed);
				yield return new WaitForEndOfFrame();
				if (Vector3.Distance(transform.position, (_isInScreenSpace?Coordinate.instance.Screen2Space(_destination):_destination)) < Deviation) break;
			}
			if(followCamera) EnableFollowCamera();
			_runningCoroutine = null;
		}
	}
}
