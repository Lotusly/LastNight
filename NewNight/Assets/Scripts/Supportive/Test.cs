using System.Collections.Generic;
using Ui;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

namespace Supportive
{
	public class Test : Singleton<Test>
	{
		public int index=1;
		private Image [] _images;
		public PlayableAsset timeLine;
		public GameObject[] trackObjects;
		private TransitionForm.TransitionParameterBlock _transitionParameters;
		private string sceneName;
		private int sceneIndex = 1;
		
		void Start()
		{
			_images = GetComponentsInChildren<Image>();
		}

		public void ShowImage(int index)
		{
			_images[index].enabled = true;
		}

		private void performLayer()
		{

			PlayableDirector director = GetComponent<PlayableDirector>();
			if(director==null) director=gameObject.AddComponent<PlayableDirector>();

			director.playOnAwake = false;
			director.extrapolationMode = DirectorWrapMode.Hold;
			director.playableAsset = Instantiate(timeLine);
			//director.SetGenericBinding("0",trackObjects[0]);
			int i = 0;
			foreach(var tr in director.playableAsset.outputs)
			{
				director.SetGenericBinding(tr.sourceObject,trackObjects[i]);
				i++;
			}
			director.Play();
		}
		public void Transfer2()
		{
			//transform.position = Coordinate.instance.transform.position;

			TransitionForm.instance.ClearParameter(ref _transitionParameters);
			TransitionForm.instance.SetBackgroundParameters(ref _transitionParameters,2,new Vector3(2,0,30), true);
			TransitionForm.instance.SetForegroundParameters(ref _transitionParameters, 2, new Vector3(2,0,13.53f), true );
			UiManager.instance.FadeOutPresentScene(_transitionParameters);

			sceneName="scene" + sceneIndex.ToString();
			sceneIndex++;

			UiManager.instance.CreateScene(sceneName);
			UiManager.instance.GenerateInScene(sceneName,"Backgrounds/1", new Vector3(-2, 0, 30), true, "Background");
			UiManager.instance.GenerateInScene(sceneName,"Characters/3", new Vector3(-2, 0, 13.53f), true, "Foreground");

			TransitionForm.instance.ClearParameter(ref _transitionParameters);
			TransitionForm.instance.SetBackgroundParameters(ref _transitionParameters,2,new Vector3(0,0,30), true);
			TransitionForm.instance.SetForegroundParameters(ref _transitionParameters, 2, new Vector3(0.75f,-0.3f,13.53f), true );
			UiManager.instance.FadeInScene(sceneName,_transitionParameters);
		}
		
		public void Transfer3()
		{
			transform.position = Coordinate.instance.transform.position;

			TransitionForm.instance.ClearParameter(ref _transitionParameters);
			TransitionForm.instance.SetBackgroundParameters(ref _transitionParameters,2,new Vector3(2,0,30), true);
			TransitionForm.instance.SetForegroundParameters(ref _transitionParameters, 2, new Vector3(2,0,13.53f), true );
			UiManager.instance.FadeOutPresentScene(_transitionParameters);

			sceneName="scene" + sceneIndex.ToString();
			sceneIndex++;

			UiManager.instance.CreateScene(sceneName);
			UiManager.instance.GenerateInScene(sceneName,"Backgrounds/1", new Vector3(-2, 0, 30), true, "Background");
			UiManager.instance.GenerateInScene(sceneName,"Characters/3", new Vector3(-2, 0, 13.53f), true, "Foreground");

			TransitionForm.instance.ClearParameter(ref _transitionParameters);
			TransitionForm.instance.SetBackgroundParameters(ref _transitionParameters,4,new Vector3(0,0,0), true);
			TransitionForm.instance.SetForegroundParameters(ref _transitionParameters, 2, new Vector3(0.75f,-0.3f,13.53f), true );
			UiManager.instance.FadeInScene(sceneName,_transitionParameters);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				UiManager.instance.ZoomOut();
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				index = (index + 1) % 2;
				UiManager.instance.SwitchBackground(index);
			}

			if (Input.GetMouseButtonDown(0))
			{
				Transfer3();
			}

			if (Input.GetKeyDown(KeyCode.J))
			{
				performLayer();
			}

			/*if (Input.GetMouseButtonDown(0))
			{
				for (int i = 0; i < 10; i++)
				{
					UiManager.instance.Generate("Midground", 0, new Vector3(-1f+i*0.2f,0,6), true, false);

				}
				Vector2 direction = new Vector2(2*Input.mousePosition.x/Screen.width-1,2*Input.mousePosition.y/Screen.height-1);
				//print(direction);
				UiManager.instance.SwitchBackground(index,direction);
			}*/



			// the block below is to test Coordinate class and UiManager.Generate and UiItem.Transfer
			/*

			[SerializeField] private string pathName;
			[SerializeField] private int _index;
			[SerializeField] private Vector3 _spaceInScreen;
			[SerializeField] private List<GameObject> _objects;


			[SerializeField] private bool _generate = false;
			[SerializeField] private bool _eliminate = false;
			[SerializeField] private bool _transfer = false;
			[SerializeField] private bool _clearAll = false;

			if (_generate)
			{
				_generate = false;
				GameObject newItem = Ui.UiManager.instance.Generate(pathName,_index, _spaceInScreen, true).gameObject;
				_objects.Add(newItem);

			}

			if (_eliminate)
			{
				_eliminate = false;
				Destroy(_objects[_index]);
				_objects.RemoveAt(_index);

			}

			if (_transfer)
			{
				_transfer = false;
				_objects[_index].GetComponent<UiItem>().Transfer(Coordinate.instance.Screen2Space(_spaceInScreen),true,true);
			}

			if (_clearAll)
			{
				_clearAll = false;
				for (int i = 0; i < _objects.Count; i++)
				{
					Destroy(_objects[i]);
				}
				_objects.Clear();
			}*/
		}
	}
}
