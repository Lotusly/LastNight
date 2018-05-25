using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ui
{

	public class UiManager : Supportive.Singleton<UiManager>
	{

		// later should replace with a few different parents
		[SerializeField] private Transform _parent;
		[SerializeField] private Transform _portraits;
		[SerializeField] private Transform _characters;
		[SerializeField] private Transform _options;
		[SerializeField] private Transform _descriptions;
		[SerializeField] private Transform _frontDecorations;
		[SerializeField] private Transform _backDecorations;
		[SerializeField] private Transform _backgrounds;


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
			newItem.transform.parent = _parent;
			newItem.transform.position = positionInWorld;
			return newItem;

		}

		public void ZoomIn()
		{
			Camera.main.gameObject.GetComponent<UiItem>().Transfer(new Vector3(0,0,3),false);
		}

		public void ZoomOut()
		{
			Camera.main.gameObject.GetComponent<UiItem>().Transfer(new Vector3(0,0,0),false);
		}
	}
}
