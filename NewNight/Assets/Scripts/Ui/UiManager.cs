using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Ui
{

	public class UiManager : Supportive.Singleton<UiManager>
	{

		[SerializeField] private Transform _generalParent;
		[SerializeField] private Transform _characters;
		[SerializeField] private Transform _portraits;
		[SerializeField] private Transform _options;
		[SerializeField] private Transform _descriptions;
		[SerializeField] private Transform _frontDecorations;
		[SerializeField] private Transform _backDecorations;
		[SerializeField] private Transform _backgrounds;
		[SerializeField] private UiCamera _camera;

		private bool exitable = false;
		private Dictionary<string, Transform> _nameToParent;

		void Start()
		{
			_nameToParent = new Dictionary<string, Transform>()
			{
				{ "Characters",_characters},
				{"BackDecorations",_backDecorations}
				
			};
			
			Story.instance.Initialize();
		}


		public GameObject Generate(string pathName, int index, Vector3 position, bool inScreenSpace)
		{
			string path="Assets/Resources/"+pathName;
			// these error detects can be replaced by neater class initialization
			if (!Directory.Exists(path))
			{
				Debug.LogError("UiManager try to generate item from unexisted directory: "+path);
				return null;
			}
			int numberOfFiles = Directory.GetFiles(path,"*.prefab").Length;
			if (numberOfFiles < 1)
			{
				Debug.LogError("UiManager try to generate an item from empty directory: "+path);
				return null;
			}

			if (index < 0)
			{
				index = Random.Range(0,numberOfFiles);
			}

			path = path + "/" + index.ToString()+".prefab";
			
			if (!File.Exists(path))
			{
				Debug.LogError("UiManager try to generate an unexisted item: "+path);
				return null;
			}
			
			

			Vector3 positionInWorld = position;
			if (inScreenSpace)
			{
				positionInWorld = Coordinate.instance.Screen2Space(positionInWorld);
			}

			GameObject newItem = Instantiate(Resources.Load<GameObject>(pathName+"/"+index.ToString()));
			newItem.transform.position = positionInWorld;
			
			if (!_nameToParent.ContainsKey(pathName))
			{
				Debug.LogWarning("UiManager generates an item that doesn't have registered  category/parent: "+pathName);
				newItem.transform.parent = _generalParent;
			}
			else
			{
				newItem.transform.parent = _nameToParent[pathName];
			}
			return newItem;

		}

		

		public void ZoomIn(Character focus)
		{
			exitable = true;
			_camera.Transfer(new Vector3(0,0,3),false,false);
			focus.transform.parent = _portraits;
			focus.Transfer(new Vector3(-0.5f,-0.4f,5),true,false);
		}

		public void ZoomOut()
		{
			if (exitable)
			{
				exitable = false;
				_camera.MoveBack();
				MoveBackGroup(_portraits,_characters);
				MoveBackGroup(_frontDecorations);
			}

		}

		private void MoveBackGroup(Transform parent, Transform switchToParent = null)
		{
			int numChildren = parent.GetChildCount();
			for (int i = 0; i < numChildren; i++)
			{
				UiItem item = parent.GetChild(i).gameObject.GetComponent<UiItem>();
				if (item != null)
				{
					item.MoveBack();
					if (switchToParent != null) item.transform.parent = switchToParent;
				}
				else
				{
					Debug.LogError("UiManager try to MoveBackGroup a parent that has non-UiItem child");
				}
			}
		} 
	}
}
