// Reference: https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html by christophfranke123 from Unity Answers

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Supportive
{
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField] private TKey defaultKey;
		[SerializeField] private TValue defaultValue;
		[SerializeField] private List<TKey> keys = new List<TKey>();
		[SerializeField] private List<TValue> values = new List<TValue>();

		// save the dictionary to lists
		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();
			foreach (KeyValuePair<TKey, TValue> pair in this)
			{
				keys.Add(pair.Key);
				values.Add(pair.Value);
			}
		}

		// load dictionary from lists
		public void OnAfterDeserialize()
		{
			this.Clear();

			//if (keys.Count != values.Count)
				//throw new System.Exception(string.Format(
					//"there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

			for (int i = 0; i < keys.Count; i++)
			{
				if (this.ContainsKey(keys[i])) keys[i] = defaultKey;
				if (i < values.Count)
				{
					this.Add(keys[i], values[i]);
				}
				else
				{
					this.Add(keys[i],defaultValue);
				}
			}

		}
	}
}
