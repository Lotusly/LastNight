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
			[SerializeField] public StringBatchDict dict ;
		};

		
		private Dictionary<string, Scene> _sceneDict;
		private string[] _nameList;
		private string _presentSceneName="";
		[SerializeField] private Scene _voidScene;

		public static SceneManager instance;
		
		void Awake()
		{
			if (instance == null) instance = this;
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
				parent= theScene.dict[kindName].transform;
			}
			else
			{
				parent = theScene.dict["Others"].transform;
			}

			int count = parent.gameObject.GetComponentsInChildren<UiItem>().Length + 1;
			parent.position = ((count - 1) * parent.position + position) / count;
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
			newScene.dict = new StringBatchDict();
			GameObject newObject = new GameObject(name);
			newScene.SceneObject = newObject;
			newObject.transform.parent = transform;
			newObject.transform.localPosition = Vector3.zero;

			for (int i = 0; i < _nameList.Length; i++)
			{
				GameObject newChild = new GameObject(_nameList[i]);
				newChild.transform.parent = newObject.transform;
				newChild.transform.localPosition = Vector3.zero;
				newScene.dict.Add(_nameList[i], newChild.AddComponent<BatchNode>());
			}

			_sceneDict.Add(name,newScene);
			
			return true;

		}

		public bool DeleteScene(string name)
		{
			if (!_sceneDict.ContainsKey(name) || name=="VoidScene") return false;
			Destroy(_sceneDict[name].SceneObject);
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
	

		public string GetPresentSceneName()
		{
			return _presentSceneName;
		}

		public BatchNode GetPresentBackground()
		{
			return _sceneDict[_presentSceneName].dict["Background"];
		}

		public BatchNode GetBackground(string name)
		{
			return _sceneDict[name].dict["Background"];
		}

		public BatchNode GetMidground(string name)
		{
			return _sceneDict[name].dict["Midground"];
		}
		
		public BatchNode GetForeground(string name)
		{
			return _sceneDict[name].dict["Foreground"];
		}
		
		public BatchNode GetOthers(string name)
		{
			return _sceneDict[name].dict["Others"];
		}

		public bool SceneExist(string name)
		{
			return _sceneDict.ContainsKey(name);
		}
	}
}
