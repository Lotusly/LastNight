using System;
using System.Collections.Generic;
using UnityEngine;
using Supportive;

namespace Ui
{
	public class SceneManager : MonoBehaviour
	{
		[Serializable]
		public struct Scene
		{
			public GameObject SceneObject;
			[SerializeField] public StringTransformDict dict ;
		};

		
		private Dictionary<string, Scene> _sceneDict;
		private string[] _nameList;
		private string _presentSceneName="";
		[SerializeField] private Scene _voidScene;

		void Awake()
		{
			_nameList = new string[] {"Background", "Midground", "Foreground", "Others" };
			_sceneDict = new Dictionary<string, Scene>();
			_sceneDict.Add("VoidScene",_voidScene);
			_presentSceneName = "VoidScene";
		}

		public UiItem GenerateItem(string sceneName, string objectPath, Vector3 position, string kindName, bool initialize)
		{
			if (!_sceneDict.ContainsKey(sceneName)) return null;
			GameObject newObject = Resources.Load<GameObject>(objectPath);
			if (newObject == null) return null;

			Scene theScene = _sceneDict[sceneName];
			Transform parent;
			if (theScene.dict.ContainsKey(kindName))
			{
				parent= theScene.dict[kindName];
			}
			else
			{
				parent = theScene.dict["Others"];
			}

			newObject = Instantiate(newObject,parent);
			newObject.transform.position = position;

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
			
			Scene newScene = new Scene();
			newScene.dict = new StringTransformDict();
			GameObject newObject = new GameObject(name);
			newScene.SceneObject = newObject;
			newObject.transform.parent = transform;
			newObject.transform.localPosition = Vector3.zero;

			for (int i = 0; i < _nameList.Length; i++)
			{
				GameObject newChild = new GameObject(_nameList[i]);
				newChild.transform.parent = newObject.transform;
				newChild.transform.localPosition = Vector3.zero;
				newScene.dict.Add(_nameList[i], newChild.transform);
				newChild.AddComponent<BatchNode>();
			}

			_sceneDict.Add(name,newScene);
			
			return true;

		}

		public bool DeleteScene(string name)
		{
			if (!_sceneDict.ContainsKey(name)) return false;
			Destroy(_sceneDict[name].SceneObject);
			_sceneDict.Remove(name);
			return true;
		}

		public bool SwitchScene(string name)
		{
			if (!_sceneDict.ContainsKey(name)) return false;
			_presentSceneName = name;
			return true;
		}
	

		public string GetPresentSceneName()
		{
			return _presentSceneName;
		}

		public Background GetPresentBackground()
		{
			return _sceneDict[_presentSceneName].dict["Background"].GetComponentInChildren<Background>();
		}
	}
}
