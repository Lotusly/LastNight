using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Supportive;

namespace Ui
{
	public class SceneManager : MonoBehaviour
	{

		private Dictionary<string, Scene> _sceneDict;
		private string[] _nameList;
		private string _presentSceneName = "";
		[SerializeField] private Scene _voidScene;

		public static SceneManager instance;

		void Awake()
		{
			if (instance == null) instance = this;
			_nameList = new string[] {"Background", "Midground", "Foreground", "Others"};
			_sceneDict = new Dictionary<string, Scene>();
			_sceneDict.Add("VoidScene", _voidScene);
			_presentSceneName = "VoidScene";
		}

		public UiItem GenerateItem(string sceneName, string objectPath, Vector3 position, string kindName,
			bool initialize)
		{
			if (!_sceneDict.ContainsKey(sceneName)) return null;
			GameObject newObject = Resources.Load<GameObject>(objectPath);
			if (newObject == null) return null;

			Scene theScene = _sceneDict[sceneName];
			BatchNode parentNode;
			if (theScene.DetectSceneBatch(kindName))
			{
				parentNode = theScene.GetSceneBatch(kindName);
			}
			else
			{
				parentNode = theScene.GetSceneBatch("Others");
			}

			newObject = Instantiate(newObject, parentNode.transform);
			newObject.transform.position = position;

			// Call parentNode.UpdatePosition here, if its leaves are likely to move from original position
			parentNode.AddLeaf(newObject.transform);


			UiItem returnValue = newObject.GetComponent<UiItem>();
			if (returnValue == null)
			{
				returnValue = newObject.AddComponent<UiItem>();
			}

			if (initialize)
			{
				returnValue.Initialize();
			}

			return newObject.GetComponent<UiItem>();
		}



		public bool NewScene(string name)
		{
			if (name == "") return false;
			if (_sceneDict.ContainsKey(name)) return false;

			
			GameObject newObject = new GameObject(name);
			Scene newScene = newObject.AddComponent<Scene>();
			newObject.transform.parent = transform;
			newObject.transform.localPosition = Vector3.zero;

			for (int i = 0; i < _nameList.Length; i++)
			{
				GameObject newChild = new GameObject(_nameList[i]);
				newChild.transform.parent = newObject.transform;
				newChild.transform.localPosition = Vector3.zero;
				newScene.AddSceneBatch(_nameList[i], newChild.AddComponent<BatchNode>());
			}

			_sceneDict.Add(name, newScene);

			return true;

		}

		public bool DeleteScene(string name)
		{
			if (!_sceneDict.ContainsKey(name) || name == "VoidScene") return false;
			Destroy(_sceneDict[name].gameObject);
			_sceneDict.Remove(name);
			if (name == _presentSceneName)
			{
				_presentSceneName = "VoidScene";
			}

			return true;
		}

		public bool SwitchScene(string name)
		{
			if (!_sceneDict.ContainsKey(name)) return false;
			_presentSceneName = name;
			return true;
		}

		public void TransferScene(string name, TransitionForm.TransitionParameterBlock block, bool destroy = false)
		{
			if (name == "VoidScene" && destroy || !_sceneDict.ContainsKey(name)) return;

			
			TransitionForm.instance.PerformTransition(name,block, destroy);
			
			
			
		}


		public string GetPresentSceneName()
		{
			return _presentSceneName;
		}

		public BatchNode GetPresentBackground()
		{
			return _sceneDict[_presentSceneName].GetSceneBatch("Background");
		}

		public BatchNode GetBackground(string name)
		{
			return _sceneDict[name].GetSceneBatch("Background");
		}

		public BatchNode GetMidground(string name)
		{
			return _sceneDict[name].GetSceneBatch("Midground");
		}
		
		public BatchNode GetForeground(string name)
		{
			return _sceneDict[name].GetSceneBatch("Foreground");
		}
		
		public BatchNode GetOthers(string name)
		{
			return _sceneDict[name].GetSceneBatch("Others");
		}

		public bool SceneExist(string name)
		{
			return _sceneDict.ContainsKey(name);
		}
	}
}
