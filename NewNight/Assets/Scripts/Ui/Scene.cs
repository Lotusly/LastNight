using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Supportive;

namespace Ui
{
	/// <summary>
	/// Each Scene (Shot in Elliot's system) can contain BatchNode_s in its dictionary variable dict.
	/// In present system, dict contains 4 keys: "Background" "Foreground" "Midground" "Others"
	/// This class just provides functions to manage the dict.
	/// </summary>
	public class Scene : BatchNode
	{
		[SerializeField] private StringBatchDict dict;

		void Awake()
		{
			dict=new StringBatchDict();
		}

		public bool RemoveSceneBatch(string name)
		{
			if (dict.ContainsKey(name))
			{
				Destroy(dict[name].gameObject);
				dict.Remove(name);
				return true;
			}

			return false;
		}

		public bool AddSceneBatch(string name, BatchNode batch)
		{
			if (!dict.ContainsKey(name))
			{
				dict.Add(name, batch);
				return true;
			}

			return false;
		}
		
	

		public BatchNode GetSceneBatch(string name)
		{
			return dict[name];
		}

		public bool DetectSceneBatch(string name)
		{
			return dict.ContainsKey(name);
		}

		public void DestroyOnEmpty()
		{
			if (dict.Count == 0)
			{
				SceneManager.instance.DeleteScene(name);
			}

		}
	
	}
}
