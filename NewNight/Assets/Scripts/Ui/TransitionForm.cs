using System.Collections;
using UnityEngine;
using Supportive;

namespace Ui
{

	public class TransitionForm : MonoBehaviour
	{
		
		//------------------------------PARAMETER BLOCK--------------------------------------------
		public struct TransitionParameterBlock
		{
			public int BackgroundMethod;
			public Vector3 BackgroundPosition;
			public bool BackgroundInScreen;
			public float BackgroundSpeed;
			public float BackgroundDelay;
			
			public int MidgroundMethod;
			public Vector3 MidgroundPosition;
			public bool MidgroundInScreen;
			public float MidgroundSpeed;
			public float MidgroundDelay;
			
			public int ForegroundMethod;
			public Vector3 ForegroundPosition;
			public bool ForegroundInScreen;
			public float ForegroundSpeed;
			public float ForegroundDelay;
			
			public int BackgroundCameraMethod;
			public Vector3 BackgroundCameraPosition;
			public float BackgroundCameraSpeed;
			public float BackgroundCameraDelay;
			
			public int MainCameraMethod;
			public Vector3 MainCameraPosition;
			public float MainCameraSpeed;
			public float MainCameraDelay;
			
			public int OthersMethod;
			public Vector3 OthersPosition;
			public bool OthersInScreen;
			public float OthersSpeed;
			public float OthersDelay;
		};

		public void ClearParameter(ref TransitionParameterBlock block)
		{
			block.BackgroundMethod = 0;
			block.MidgroundMethod = 0;
			block.ForegroundMethod = 0;
			block.BackgroundCameraMethod = 0;
			block.MainCameraMethod = 0;
			block.OthersMethod = 0;

			block.BackgroundPosition = Vector3.zero;
			block.MidgroundPosition = Vector3.zero;
			block.ForegroundPosition = Vector3.zero;
			block.BackgroundCameraPosition = Vector3.zero;
			block.MainCameraPosition = Vector3.zero;
			block.OthersPosition = Vector3.zero;

			block.BackgroundInScreen = false;
			block.MidgroundInScreen = false;
			block.ForegroundInScreen = false;
			block.OthersInScreen = false;

			block.BackgroundSpeed = 1;
			block.MidgroundSpeed = 1;
			block.ForegroundSpeed = 1;
			block.BackgroundCameraSpeed = 1;
			block.MainCameraSpeed = 1;
			block.OthersSpeed = 1;

			block.BackgroundCameraDelay = 0;
			block.BackgroundDelay = 0;
			block.ForegroundDelay = 0;
			block.MainCameraDelay = 0;
			block.MidgroundDelay = 0;
			block.OthersDelay = 0;

		}

		public void SetBackgroundParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1, float delay = 0)
		{
			block.BackgroundMethod = method;
			block.BackgroundPosition = newPosition;
			block.BackgroundInScreen = inScreen;
			block.BackgroundSpeed = speed;
			block.BackgroundDelay = delay;
		}
		
		public void SetMidgroundParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1, float delay = 0)
		{
			block.MidgroundMethod = method;
			block.MidgroundPosition = newPosition;
			block.MidgroundInScreen = inScreen;
			block.MidgroundSpeed = speed;
			block.MidgroundDelay = delay;
		}
		
		public void SetForegroundParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1, float delay = 0)
		{
			block.ForegroundMethod = method;
			block.ForegroundPosition = newPosition;
			block.ForegroundInScreen = inScreen;
			block.ForegroundSpeed = speed;
			block.ForegroundDelay = delay;
		}
		
		public void SetBackgroundCameraParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1, float delay = 0)
		{
			block.BackgroundCameraMethod = method;
			block.BackgroundCameraPosition = newPosition;
			block.BackgroundCameraSpeed = speed;
			block.BackgroundCameraDelay = delay;
		}
		
		public void SetMainCameraParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1, float delay = 0)
		{
			block.MainCameraMethod = method;
			block.MainCameraPosition = newPosition;
			block.MainCameraSpeed = speed;
			block.MainCameraDelay = delay;
		}
		
		//------------------------------------------MOVEMENT------------------------------------------------
		public delegate IEnumerator Movement(Transform tran, Vector3 newPosition, bool inScreen, float speed, float delay); // speed here means finish the whole movement in 1/speed seconds
		public delegate void AfterTransition();
		public Movement[] Movements;
		public static TransitionForm instance;
		void Awake()
		{
			if(instance==null)instance = this;
			Movements = new Movement[6];
			Movements[0] = Nothing;
			Movements[1] = Direct;
			Movements[2] = Average;
		}
	
		public IEnumerator Nothing(Transform tran, Vector3 newPosition, bool inScreen, float speed=1, float delay=0)
		{
			Debug.Log("Nothing");
			yield return null;
		}

		public IEnumerator Direct(Transform tran, Vector3 newPosition, bool inScreen, float speed=1, float delay=0)
		{
			Debug.Log("Direct");
			yield return new WaitForSecondsRealtime(delay);
			if (inScreen) tran.position = Coordinate.instance.Screen2Space(newPosition);
			else tran.position = newPosition;
		}

		public IEnumerator Average(Transform tran, Vector3 newPosition, bool inScreen, float speed=1, float delay=0)
		{
			Debug.Log("Average");
			yield return new WaitForSecondsRealtime(delay);
			Vector3 oriPosition;
			if (inScreen) oriPosition = Coordinate.instance.Space2Screen(tran.position);
			else oriPosition = tran.position;
			if (speed > 0)
			{
				float tMax = 1 / speed;
				Vector3 perSecond = (newPosition - oriPosition) *speed;
				float t = 0;
				Vector3 presentPosition=oriPosition;
				while (t < tMax)
				{
					if (inScreen)
					{
						presentPosition += perSecond * Time.deltaTime;
						tran.position = Coordinate.instance.Screen2Space(presentPosition);
					}
					else
					{
						tran.position += perSecond * Time.deltaTime;
					}

					t += Time.deltaTime;
					yield return new WaitForEndOfFrame();
				}
			}
		}

		public IEnumerator Lerp(Transform tran, Vector3 newPosition, bool inScreen, float speed = 1, float delay = 0)
		{
			yield return null;
			// not finished
		}
		
		//------------------------FUNCTION------------------------------------
		public void PerformTransition(string name, TransitionParameterBlock block)
		{
			StartCoroutine(Movements[block.BackgroundMethod](SceneManager.instance.GetBackground(name),
				block.BackgroundPosition,
				block.BackgroundInScreen, block.BackgroundSpeed, block.BackgroundDelay));

			StartCoroutine(Movements[block.MidgroundMethod](SceneManager.instance.GetMidground(name), block.MidgroundPosition,
				block.MidgroundInScreen, block.MidgroundSpeed, block.MidgroundDelay));
			StartCoroutine(Movements[block.ForegroundMethod](SceneManager.instance.GetForeground(name), block.ForegroundPosition,
				block.ForegroundInScreen, block.ForegroundSpeed, block.ForegroundDelay));
			StartCoroutine(Movements[block.OthersMethod](SceneManager.instance.GetOthers(name), block.OthersPosition,
				block.OthersInScreen, block.OthersSpeed, block.OthersDelay));
			// not finished
		}
		
	}
}
