using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
    // (Put comment here describing what this class is supposed to do) 

    // In general, functions should have a verb-object syntax structure 
    // i.e. DeleteScene() is a good function name, but you can change NewScene()
    // to CreateScene()
    // Only exception to this syntax structure is boolean functions, in which 
    // they should begin with words like "is" or "has"
    // i.e. SceneExist() -> IsSceneExisting()

    // This is more of my preference (you don't have to follow), but if you 
    // have public functions 
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

            // (Explain the purpose of void scene) 
			_sceneDict.Add("VoidScene", _voidScene);
			_presentSceneName = "VoidScene";
		}

        // (What is "kindName" supposed to be?) 
        // (Explain what this function is doing?)
		public UiItem GenerateItem(string sceneName, string objectPath, Vector3 position, string kindName,
			bool initialize)
		{
			if (!_sceneDict.ContainsKey(sceneName)) return null;
			GameObject newObject = Resources.Load<GameObject>(objectPath);
			if (newObject == null) return null;

            // (Explain this chunk of code) 
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

            // (If what is initialized?) 
			if (initialize)
			{
				returnValue.Initialize();
			}

			return newObject.GetComponent<UiItem>();
		}

        // (Why is this function a boolean if it's always returning true?
        // Is it possible to just make it a void function?)
		public bool NewScene(string name)
		{
			if (name == "") return false;
			if (_sceneDict.ContainsKey(name)) return false;

            // (Explain this chunk of code)
			GameObject newObject = new GameObject(name);
			Scene newScene = newObject.AddComponent<Scene>();
			newObject.transform.parent = transform;
			newObject.transform.localPosition = Vector3.zero;

            // (Explain this chunk of code)
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

        // (Explain how this function is deleting the scene; not sure what's 
        // going on with the print statements)
		public bool DeleteScene(string name)
		{
			print(name+" 0");
			if (!_sceneDict.ContainsKey(name) || name == "VoidScene") return false;
			print(name+" 1");
			Destroy(_sceneDict[name].gameObject);
			print(name+" 2");
			_sceneDict.Remove(name);
			if (name == _presentSceneName)
			{
				_presentSceneName = "VoidScene";
			}
			print(name+" 3");
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
