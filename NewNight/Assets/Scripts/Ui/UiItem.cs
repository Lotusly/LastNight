using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ui
{
	public abstract class UiItem : MonoBehaviour
	{

		private const float Deviation = 0.2f;
		protected float _speed=1f; // later make this adjustable
		[SerializeField]protected Vector3 _positionOutScreen;
		protected Vector3 _originalPosition;
		[SerializeField] protected bool _selfBoost=false;
		protected UnityEvent _afterArrival = new UnityEvent();

		private Vector3 _followingCoordinate;

		
		
		
		private Vector3 _destination;
		private bool _isInScreenSpace=false;// used in both Follow Camera and Lerp To Transfer

		protected UiItem _followingObject = null; // UiItem should have function to stay static to the camera
		
		

		private Coroutine _runningCoroutine=null; // this method can only run one animation at the same time: no addition


		
		
		void Update()
		{
			if (_followingObject!=null)
			{
				if (_isInScreenSpace)
				{
					Debug.LogError("UiItem follow object while in screen space");
					_followingObject = null;
				}
				else
				{
					transform.position = _followingObject.transform.position + _followingCoordinate;
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

		public void EnableFollowObject(UiItem item = null)
		{
			if(item == null) _followingObject = UiManager.instance._camera;
			else _followingObject = item;
			
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);

			_followingCoordinate = transform.position-_followingObject.transform.position;
			_isInScreenSpace = false;

			
		}

		private void DisableFollowObject()
		{
			_followingObject = null;
		}


		public void SetPosition(Vector3 newPosition, bool inScreenSpace, bool recordOrigin, bool followCamera = false) //new function
		{
			if (recordOrigin) _originalPosition = transform.position; // if record, then remember where it leaves; otherwise don't update
			//EnableFollowObject();
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			_destination = newPosition;
			_isInScreenSpace = inScreenSpace;
			transform.position = (inScreenSpace ? Coordinate.instance.Screen2Space(newPosition) : newPosition);
			if(followCamera) EnableFollowObject();
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
			DisableFollowObject();
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
			if(followCamera) EnableFollowObject();
			_runningCoroutine = null;
			_afterArrival.Invoke();
		}
	}
}
