using System.Collections;
using UnityEngine;
using Supportive;
using UnityEditor.ShaderGraph;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
		// TEMP
		public PlayableAsset timeLine;
		void Awake()
		{
			if(instance==null)instance = this;
			Movements = new Movement[6];
			Movements[0] = Nothing;
			Movements[1] = Direct;
			Movements[2] = Average;
			Movements[3] = Lerp;
			Movements[4] = Layers;
			Movements[5] = RadialBlur;
		}

	
		public IEnumerator Nothing(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy=false, float speed=1, float delay=0)
		{
			Debug.Log("Nothing");
			yield return null;
			if (destroy)
			{
				DestroyTran(tran);
			}
		}

		public IEnumerator Direct(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy=false, float speed=1, float delay=0)
		{
			Debug.Log("Direct");
			yield return new WaitForSecondsRealtime(delay);
			tran.SetPosition(newPosition,inScreen);
			if (destroy)
			{
				DestroyTran(tran);
			}
		}

		public IEnumerator Average(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy=false, float speed=1, float delay=0)
		{
			Debug.Log("Average");
			yield return new WaitForSecondsRealtime(delay);
			yield return StartCoroutine(tran.Transfer(newPosition, inScreen,false,1));
			if (destroy)
			{
				DestroyTran(tran);
			}
		}

		public IEnumerator Lerp(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy=false, float speed = 1, float delay = 0)
		{
			Debug.Log("Lerp");
			yield return new WaitForSecondsRealtime(delay);
			yield return StartCoroutine(tran.Transfer(newPosition, inScreen,false,0,speed));
			if (destroy)
			{
				DestroyTran(tran);
			}
		}

		

		
		/// <summary>
		/// The 4th transfer method. Cinematic Parallax fade-in. Should apply to background. Resources/Timelines/1.
		/// </summary>
		/// <param name="tran"> For background BatchNode only. There should be only 1 background under this BatchNode.</param>
		/// <param name="newPosition"> The position you want the background to be finally be.</param>
		/// <param name="inScreen"> Whether newPosition is in screen space or world space.</param>
		/// <param name="destroy"> Redundant parameter. Tran will not be destroyed anyway.</param>
		/// <param name="speed"> Redundant for now.</param>
		/// <param name="delay"> Wait for seconds before performing the fade-in.</param>
		/// <returns></returns>
		public IEnumerator Layers(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy = false,
			float speed = 1, float delay = 0)
		{
			int index = 1; // this means the transition uses Resources/Timelines/1
			int i;
			Debug.Log("Layers");
			yield return new WaitForSecondsRealtime(delay);

			Background b = tran.gameObject.GetComponentInChildren<Background>();
			if (b != null)
			{
				GameObject _original = b.gameObject;

				GameObject _base = Instantiate(_original, tran.transform);
				if(_base.GetComponent<Animator>()==null) _base.AddComponent<Animator>();
				GameObject _shadow = Instantiate(_base,tran.transform), _tone = Instantiate(_base,tran.transform),
					_highlight = Instantiate(_base,tran.transform),_highlight1 = Instantiate(_base,tran.transform), _transparent = Instantiate(_base,tran.transform);
				GameObject[] trackObjects = new GameObject[7];
				_base.transform.localScale = new Vector3(32.5f, 19, 1);
				if (_base.GetComponent<Animator>() == null) _base.AddComponent<Animator>();
				trackObjects[0] = _original;
				trackObjects[1] = _base;
				trackObjects[2] = _highlight1;
				trackObjects[3] = _highlight;
				trackObjects[4] = _shadow;
				trackObjects[5] = _tone;
				trackObjects[6] = _transparent;
				for (i = 0; i < 7; i++)
				{
					trackObjects[i].GetComponent<Renderer>().material=Resources.Load<Material>("Timelines/" + index.ToString() + "_"+(i).ToString());
				}
				
				tran.SetPosition(newPosition,inScreen);
				_original.transform.position = newPosition;
				_original.active = false;

				
				PlayableDirector director = tran.gameObject.GetComponent<PlayableDirector>();
			
				if (director == null)
				{
					director=tran.gameObject.AddComponent<PlayableDirector>();
				}

				director.playableAsset = Instantiate(Resources.Load("Timelines/"+index.ToString()) as PlayableAsset);
				director.playOnAwake = false;
				director.extrapolationMode = DirectorWrapMode.Hold;
				

				i = 0;
				foreach(var tr in director.playableAsset.outputs)
				{
					director.SetGenericBinding(tr.sourceObject,trackObjects[i+1]);
					i++;
				}
				director.Play();
				yield return new WaitForSecondsRealtime(3.4f);
				AfterLayers(tran,trackObjects);
			}

			else
			{
				tran.SetPosition(newPosition, inScreen);
				
			}

		}

		/// <summary>
		/// The 5th transfer method. RadialBlur fade-out. Should apply to background.
		/// </summary>
		/// <param name="tran"></param>
		/// <param name="newPosition"></param>
		/// <param name="inScreen"></param>
		/// <param name="destroy"> Redundant. Tran will be destroyed anyway.</param>
		/// <param name="speed"></param>
		/// <param name="delay"></param>
		/// <returns></returns>
		public IEnumerator RadialBlur(BatchNode tran, Vector3 newPosition, bool inScreen, bool destroy = true,
			float speed = 1, float delay = 0)
		{
			Debug.Log("RadialBlur");
			yield return new WaitForSecondsRealtime(delay);
			

			Background b = tran.GetComponentInChildren<Background>();
			if (b != null)
			{
				// MODIFYING
				//b.gameObject.GetComponent<Renderer>().material = 
				Renderer rend = b.gameObject.GetComponentInChildren<Renderer>();
				MaterialPropertyBlock matBlock=new MaterialPropertyBlock();
				matBlock.Clear();
				matBlock.SetTexture("tDiffuse", rend.material.mainTexture);
				rend.material = Instantiate(Resources.Load<Material>("Material/0"));
				//rend.SetPropertyBlock(matBlock);

				//matBlock.Clear();
				if (speed <= 0) speed = 1;
				float alpha = 1;
				float density = 0;
				float alphaStep = -1*speed;
				float densityStep = 2*speed;
				while (alpha > 0)
				{
					alpha += alphaStep * Time.deltaTime;
					density += densityStep * Time.deltaTime;
					matBlock.SetFloat("_alpha",alpha);
					matBlock.SetFloat("fDensity", density);
					rend.SetPropertyBlock(matBlock);
					yield return new WaitForEndOfFrame();
				}

				DestroyTran(tran);

			}
			else
			{
				tran.SetPosition(newPosition,inScreen);
				DestroyTran(tran);
			}

		}
		
		/// <summary>
		/// Called automatically after Layers fade-in, or when the fade-in is manually stopped.
		/// </summary>
		private void AfterLayers(BatchNode tran, GameObject[] trackObjects)
		{
			trackObjects[0].active = true;
			for (int i = 1; i < trackObjects.Length; i++)
			{
				Destroy(trackObjects[i]);
			}
			Destroy(tran.GetComponent<PlayableDirector>());
		}

		private void DestroyTran(BatchNode tran)
		{
			Scene theScene = tran.transform.parent.gameObject.GetComponent<Scene>();
			if (theScene == null)
			{
				Debug.LogError("TransitionForm.cs DestroyTran: parent doesn't have Scene component");
			}
			else
			{
				theScene.RemoveSceneBatch(tran.name);
				theScene.DestroyOnEmpty();
			}
		}
		
		//------------------------FUNCTION------------------------------------
		/// <summary>
		/// At one time, there should not be more than one transition performed on the same scene. (No interruption protection)
		/// </summary>
		/// <param name="name"></param>
		/// <param name="block"></param>
		/// <param name="destroy"></param>
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
