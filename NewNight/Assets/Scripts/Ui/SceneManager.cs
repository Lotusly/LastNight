using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ui
{
	public class SceneManager : MonoBehaviour
	{
		public struct Scene
		{
			public GameObject SceneObject;
			public Dictionary<string, Transform> dict ;
		};

		private Dictionary<string, Scene> _sceneDict;
		private string[] _nameList;

		void Awake()
		{
			_nameList = new string[] {"Background", "Midground", "Foreground", "Others" };
			_sceneDict = new Dictionary<string, Scene>();
			NewScene("VoidScene");
		}

		public bool NewScene(string name)
		{
			if (_sceneDict.ContainsKey(name)) return false;
			
			Scene newScene = new Scene();
			newScene.dict = new Dictionary<string, Transform>();
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

		public void AddItem()
		{
			
		}
	}
}
