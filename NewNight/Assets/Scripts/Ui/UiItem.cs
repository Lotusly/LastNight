using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Ui
{
    /// <summary>
    /// UiItem is the base class of many UI classes. It handles many UI functions,
    /// including the item position, state, and more.
    /// </summary>
	public class UiItem : MonoBehaviour
	{

        /// <summary>
        /// Stores information on the state of the UiItem.
        /// </summary>
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


        /// <summary>
        /// Called after the item is initialized. It handles the initial state setting.
        /// </summary>
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

        /// <summary>
        /// Updates an existing state.
        /// </summary>
        /// <param name="info">The state to be updated.</param>
        /// <param name="inScreen">False by default.</param>
        /// <param name="follow">False by default.</param>
		protected void UpdateState(ref StateInfo info, bool inScreen = false, bool follow = false)
		{
			info.Position = inScreen ? Coordinate.instance.Space2Screen(transform.position) : transform.position;
			info.InScreen = inScreen;
			info.FollowCamera = follow;
			info.Stable = true;
		}

        /// <summary>
        /// Updates an existing state by making it a duplicate of another existing state.
        /// </summary>
        /// <param name="des">The state to change</param>
        /// <param name="sor">The state to duplicate</param>
		private void DuplicateState(ref StateInfo des, StateInfo sor)
		{
			des.Position = sor.Position;
			des.InScreen = sor.InScreen;
			des.FollowCamera = sor.FollowCamera;
			des.Stable = sor.Stable;
		}
		
		/// <summary>
        /// Handles camera following if it is activated and the state is currently stable.
        /// </summary>
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

        /// <summary>
        /// if any child class of UiItem wants to initialize the original states in screen space
        /// it should do it in this function. Because it is virtual it needs to be overridden by
        /// child classes.
        /// </summary>
        /// <param name="aimPosition">the starting position of the UI item.</param>
		public virtual void Initialize(Vector3 aimPosition = new Vector3()) {}

        /// <summary>
        /// Not being used anymore? Because it is virtual it needs to be overridden by child classes.
        /// </summary>
        /// <param name="focus"></param>
		public virtual void MoveOut(UiItem focus = null) {}

        /// <summary>
        /// Not being used anymore? Because it is virtual it needs to be overridden by child classes.
        /// </summary>
        public virtual void MoveBack()
		{
			Transfer(lastState.Position,lastState.InScreen);
		}

        /// <summary>
        /// Enables camera following on the current state (which only works when the state is currently stable).
        /// </summary>
		public void EnableFollowCamera()
		{
			
			if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);
			presentState.InScreen = true;
			presentState.Position = Coordinate.instance.Space2Screen(transform.position);
			presentState.FollowCamera = true;
			presentState.Stable = true;
			// follow camera only work when stable


		}

        /// <summary>
        /// Disables camera following on the current state.
        /// </summary>
		private void DisableFollowCamera()
		{
			presentState.FollowCamera = false;
		}


        /// <summary>
        /// Sets the item to a new position and updates the current state.
        /// </summary>
        /// <param name="newPosition">The position to set the item to.</param>
        /// <param name="inScreenSpace">The new screenspace state.</param>
        /// <param name="followCamera">The new follow camera state.</param>
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

        /// <summary>
        /// Moves to new position gradually, using 1 of 3 different movement modes.
        /// </summary>
        /// <param name="newPosition">The position to move to.</param>
        /// <param name="inScreenSpace">The screen space state</param>
        /// <param name="followCamera">The follow camera state</param>
        /// <param name="mode">The mode of movement to the new position</param>
        /// <param name="speed">The speed of movement</param>
        /// <returns></returns>
		public IEnumerator Transfer(Vector3 newPosition, bool inScreenSpace, bool followCamera = false,
			int mode = 0, float speed = 1.5f, Ease ease = Ease.Linear)
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
			
			//if(_runningCoroutine!=null) StopCoroutine(_runningCoroutine);

			transform.DOMove(newPosition, speed).SetEase(ease).OnComplete(Arrival);
			yield return null;
		/*
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
		*/
		}

		private IEnumerator PlainLerp(float speed)
		{
			yield return null;

			Ease ease;
			transform.DOMove((presentState.InScreen?Coordinate.instance.Screen2Space(presentState.Position):presentState.Position), speed);
			transform.DOKill(false);

			/*
			while (true)
			{
				transform.position = Vector3.Lerp(transform.position, (presentState.InScreen?Coordinate.instance.Screen2Space(presentState.Position):presentState.Position),Time.deltaTime*speed);
				yield return new WaitForEndOfFrame();
				if (Vector3.Distance(transform.position, (presentState.InScreen?Coordinate.instance.Screen2Space(presentState.Position):presentState.Position)) < Deviation) break;
			}
			*/

			if(presentState.FollowCamera) EnableFollowCamera();
			AfterArrival.Invoke();
			_runningCoroutine = null;
			
		}

		private void Arrival()
		{
			AfterArrival.Invoke();
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
