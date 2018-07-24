using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ui
{
	public class UiItem : MonoBehaviour
	{

		public struct StateInfo
		{
			public Vector3 Position;
			public bool InScreen;
			public bool FollowCamera;
			public bool Stable;
		};
		
		private const float Deviation = 0.2f;
		protected float _speed=1f; // later make this adjustable
		[SerializeField]protected Vector3 _positionOutScreen;
		//protected Vector3 _originalPosition;
		[SerializeField] protected bool _selfBoost=false;
		public UnityEvent AfterArrival = new UnityEvent();

		//private Vector3 _followingCoordinate;
		protected StateInfo lastState;
		protected StateInfo presentState;

		
		
		
		//private Vector3 _destination;
		//private bool _isInScreenSpace=false;// used in both Follow Camera and Lerp To Transfer

		//protected UiItem _followingObject = null; // UiItem should have function to stay static to the camera
		
		

		private Coroutine _runningCoroutine=null; // this method can only run one animation at the same time: no addition

		void Start()
		{
			
			lastState = new StateInfo();
			presentState = new StateInfo();
			UpdateState(ref lastState);
			UpdateState(ref presentState);
			
			
			if (_selfBoost)
			{
				Initialize(); // Initialize() should properly update state infos. should modify later
			}

			
		}

		protected void UpdateState(ref StateInfo info, bool inScreen = false, bool follow = false)
		{
			info.Position = inScreen ? Coordinate.instance.Space2Screen(transform.position) : transform.position;
			info.InScreen = inScreen;
			info.FollowCamera = follow;
			info.Stable = true;
		}

		private void DuplicateState(ref StateInfo des, StateInfo sor)
		{
			des.Position = sor.Position;
			des.InScreen = sor.InScreen;
			des.FollowCamera = sor.FollowCamera;
			des.Stable = sor.Stable;
		}
		
		
		void Update() 
		{
			if (presentState.FollowCamera && presentState.Stable)
			{
				if (presentState.InScreen)
				{
					transform.position = Coordinate.instance.Screen2Space(presentState.Position);
				}
				else
				{
					Debug.LogError(this.name+" UiItem.cs Update(): FollowCamera=true, InScreen=false.");
					presentState.FollowCamera = false;
				}
			}
		}

		public virtual void Initialize(Vector3 aimPosition = new Vector3()) {
			// if any child class of UiItem wants to initialize the original states in screen space
			// it should do it in Initialize function
			
		}




		public virtual void MoveOut(UiItem focus = null) {} 


		public virtual void MoveBack()
		{
			Transfer(lastState.Position,lastState.InScreen);
		}

		public void EnableFollowCamera()
		{
			
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			presentState.InScreen = true;
			presentState.Position = Coordinate.instance.Space2Screen(transform.position);
			presentState.FollowCamera = true;
			presentState.Stable = true;
			// follow camera only work when stable


		}

		private void DisableFollowCamera()
		{
			presentState.FollowCamera = false;
		}


		public void SetPosition(Vector3 newPosition, bool inScreenSpace,  bool followCamera = false) 
		{
			DisableFollowCamera();
			DuplicateState(ref lastState,presentState);
			
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			presentState.Position = newPosition;
			presentState.InScreen = inScreenSpace;
			if(followCamera) EnableFollowCamera(); 
			// for SetPosition, the state is stable directly.
			
			/*_destination = newPosition;
			_isInScreenSpace = inScreenSpace;*/
			transform.position = (inScreenSpace ? Coordinate.instance.Screen2Space(newPosition) : newPosition);
			//if(followCamera) EnableFollowObject();
		}

		



		public IEnumerator Transfer(Vector3 newPosition, bool inScreenSpace, bool followCamera = false,
			int mode = 0, float speed = 1.5f)
		{
			//if (recordOrigin) _originalPosition = transform.position; // if record, then remember where it leaves; otherwise don't update
			DisableFollowCamera();
			/*_destination = newPosition;
			_isInScreenSpace = inScreenSpace;*/
			DuplicateState(ref lastState,presentState);
			presentState.Position = newPosition;
			presentState.InScreen = inScreenSpace;
			presentState.FollowCamera = followCamera;
			presentState.Stable = false;
			// for Transfer, the state is not stable at first
			
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			
			switch (mode)
			{
				case 1:
					yield return _runningCoroutine = StartCoroutine(ConstantTransfer(speed));
					break;
				case 0:
					yield return _runningCoroutine = StartCoroutine(PlainLerp(speed));
					break;
				case 2:
					yield return _runningCoroutine = StartCoroutine(AccelerateTransfer(speed,1f));
					break;
			}

		}

		private IEnumerator PlainLerp(float speed)
		{
			yield return null;
			while (true)
			{
				transform.position = Vector3.Lerp(transform.position, (presentState.InScreen?Coordinate.instance.Screen2Space(presentState.Position):presentState.Position),Time.deltaTime*speed);
				yield return new WaitForEndOfFrame();
				if (Vector3.Distance(transform.position, (presentState.InScreen?Coordinate.instance.Screen2Space(presentState.Position):presentState.Position)) < Deviation) break;
			}
			if(presentState.FollowCamera) EnableFollowCamera();
			AfterArrival.Invoke();
			_runningCoroutine = null;
			
		}

		private IEnumerator ConstantTransfer(float speed)
		{
			yield return null;
			float scope = ((presentState.InScreen ? Coordinate.instance.Screen2Space(presentState.Position) : presentState.Position) -
			               transform.position).magnitude;
			speed *= scope;
			while (true)
			{
				transform.position +=( (presentState.InScreen?Coordinate.instance.Screen2Space(presentState.Position):presentState.Position) - transform.position).normalized * Time.deltaTime * speed;
				yield return new WaitForEndOfFrame();
				if (Vector3.Distance(transform.position,
					    presentState.InScreen ? Coordinate.instance.Screen2Space(presentState.Position) : presentState.Position) <
				    Deviation*scope*0.1f) break;
			}
			if(presentState.FollowCamera) EnableFollowCamera();
			AfterArrival.Invoke();
			_runningCoroutine = null;
			
		}
		
		private IEnumerator AccelerateTransfer(float speedMax, float acce)
		{
			yield return null;
			float scope = ((presentState.InScreen ? Coordinate.instance.Screen2Space(presentState.Position) : presentState.Position) -
			               transform.position).magnitude;
			speedMax *= scope;
			float speed = 0;
			
			while (true)
			{
				if(speed<speedMax) speed += Time.deltaTime * acce*scope;
				transform.position +=( (presentState.InScreen?Coordinate.instance.Screen2Space(presentState.Position):presentState.Position) - transform.position).normalized * Time.deltaTime * speed;
				yield return new WaitForEndOfFrame();
				if (Vector3.Distance(transform.position,
					    presentState.InScreen ? Coordinate.instance.Screen2Space(presentState.Position) : presentState.Position) <
				    Deviation*scope) break;
			}
			if(presentState.FollowCamera) EnableFollowCamera();
			AfterArrival.Invoke();
			_runningCoroutine = null;
			
		}

		public void DestroyHandler()
		{
			Destroy(gameObject);
		}
	}
}
