using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Supportive;

namespace Ui
{
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
