using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public abstract class UiItem : MonoBehaviour
	{

		private const float Deviation = 0.1f;
		private float _speed=1f; // later make this adjustable
		private Vector3 _originalPosition;
		
		
		private Vector3 _destination;
		private bool _isInScreenSpace=false;// used in both Follow Camera and Lerp To Transfer

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

		public abstract void Initialize(Vector3 aimPosition=new Vector3()); // new function
		


		public Vector3 GetOriginalPosition()
		{
			return _originalPosition;
		}

		public Vector3 GetDestination()
		{
			return _destination;
		}

		public virtual void MoveOut(UiItem focus = null) {} // new function


		public virtual void MoveBack()
		{
			Transfer(_originalPosition,false,false);
		}

		private void EnableFollowCamera()
		{
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			_destination = _isInScreenSpace ? _destination : Coordinate.instance.Space2Screen(_destination);
			_isInScreenSpace = true;
			_followingCamera = true;
			
		}

		private void DisableFollowCamera()
		{
			_followingCamera = false;
		}


		public void SetPosition(Vector3 newPosition, bool inScreenSpace, bool recordOrigin, bool followCamera = false) //new function
		{
			if (recordOrigin) _originalPosition = transform.position; // if record, then remember where it leaves; otherwise don't update
			DisableFollowCamera();
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			_destination = newPosition;
			_isInScreenSpace = inScreenSpace;
			transform.position = (inScreenSpace ? Coordinate.instance.Screen2Space(newPosition) : newPosition);
			if(followCamera) EnableFollowCamera();
		}

		public void UpdateOriginPosition() //new function
		{
			_originalPosition = transform.position;
		}

		public void SetOriginPosition(Vector3 position, bool inScreenSpace)
		{
			_originalPosition = inScreenSpace?Coordinate.instance.Screen2Space(position):position;
		}


		public void Transfer(Vector3 newPosition, bool inScreenSpace, bool recordOrigin, bool followCamera = false)
		{
			if (recordOrigin) _originalPosition = transform.position; // if record, then remember where it leaves; otherwise don't update
			DisableFollowCamera();
			_destination = newPosition;
			_isInScreenSpace = inScreenSpace;
			
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
