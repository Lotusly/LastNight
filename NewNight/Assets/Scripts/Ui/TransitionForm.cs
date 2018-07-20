using System.Collections;
using UnityEngine;
using Supportive;
using UnityEngine.Events;

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
		public delegate IEnumerator Movement(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy, float speed, float delay); // speed here means finish the whole movement in 1/speed seconds
		public Movement[] Movements;
		public static TransitionForm instance;
		void Awake()
		{
			if(instance==null)instance = this;
			Movements = new Movement[6];
			Movements[0] = Nothing;
			Movements[1] = Direct;
			Movements[2] = Average;
			Movements[3] = Lerp;
		}

	
		public IEnumerator Nothing(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy=false, float speed=1, float delay=0)
		{
			Debug.Log("Nothing");
			yield return null;
			if (destroy)
			{
				Scene theScene = tran.transform.parent.gameObject.GetComponent<Scene>();
				if (theScene == null)
				{
					Debug.LogError("TransitionForm.cs Average: parent doesn't have Scene component");
				}
				else
				{
					theScene.RemoveSceneBatch(tran.name);
					theScene.DestroyOnEmpty();
				}
			}
		}

		public IEnumerator Direct(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy=false, float speed=1, float delay=0)
		{
			Debug.Log("Direct");
			yield return new WaitForSecondsRealtime(delay);
			tran.SetPosition(newPosition,inScreen);
			if (destroy)
			{
				Scene theScene = tran.transform.parent.gameObject.GetComponent<Scene>();
				if (theScene == null)
				{
					Debug.LogError("TransitionForm.cs Average: parent doesn't have Scene component");
				}
				else
				{
					theScene.RemoveSceneBatch(tran.name);
					theScene.DestroyOnEmpty();
				}
			}
		}

		public IEnumerator Average(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy=false, float speed=1, float delay=0)
		{
			Debug.Log("Average");
			yield return new WaitForSecondsRealtime(delay);
			yield return StartCoroutine(tran.Transfer(newPosition, inScreen,false,1));
			if (destroy)
			{
				Scene theScene = tran.transform.parent.gameObject.GetComponent<Scene>();
				if (theScene == null)
				{
					Debug.LogError("TransitionForm.cs Average: parent doesn't have Scene component");
				}
				else
				{
					theScene.RemoveSceneBatch(tran.name);
					theScene.DestroyOnEmpty();
				}
			}
		}

		public IEnumerator Lerp(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy=false, float speed = 1, float delay = 0)
		{
			Debug.Log("Lerp");
			yield return new WaitForSecondsRealtime(delay);
			yield return StartCoroutine(tran.Transfer(newPosition, inScreen));
			if (destroy)
			{
				Scene theScene = tran.transform.parent.gameObject.GetComponent<Scene>();
				if (theScene == null)
				{
					Debug.LogError("TransitionForm.cs Average: parent doesn't have Scene component");
				}
				else
				{
					theScene.RemoveSceneBatch(tran.name);
					theScene.DestroyOnEmpty();
				}
			}
		}
		
		//------------------------FUNCTION------------------------------------
		public void PerformTransition(string name, TransitionParameterBlock block, bool destroy) 
			// this function should only be called by SceneManager, cause void scene should never be destroyed
		{
			
			/*BatchNode tmpNode;
			
			tmpNode = SceneManager.instance.GetBackground(name);
			tmpNode.AfterArrival.AddListener(tmpNode.DestroyHandler);
			
			tmpNode = SceneManager.instance.GetMidground(name);
			tmpNode.AfterArrival.AddListener(tmpNode.DestroyHandler);
			
			tmpNode = SceneManager.instance.GetForeground(name);
			tmpNode.AfterArrival.AddListener(tmpNode.DestroyHandler);
			
			tmpNode = SceneManager.instance.GetOthers(name);
			tmpNode.AfterArrival.AddListener(tmpNode.DestroyHandler);*/
			
			StartCoroutine(Movements[block.BackgroundMethod](SceneManager.instance.GetBackground(name),
				block.BackgroundPosition,
				block.BackgroundInScreen, destroy, block.BackgroundSpeed, block.BackgroundDelay));

			StartCoroutine(Movements[block.MidgroundMethod](SceneManager.instance.GetMidground(name), block.MidgroundPosition,
				block.MidgroundInScreen, destroy, block.MidgroundSpeed, block.MidgroundDelay));
			StartCoroutine(Movements[block.ForegroundMethod](SceneManager.instance.GetForeground(name), block.ForegroundPosition,
				block.ForegroundInScreen, destroy, block.ForegroundSpeed, block.ForegroundDelay));
			StartCoroutine(Movements[block.OthersMethod](SceneManager.instance.GetOthers(name), block.OthersPosition,
				block.OthersInScreen, destroy, block.OthersSpeed, block.OthersDelay));
			// not finished. Still need to deal with cameras
		}

		
		
	}
}
