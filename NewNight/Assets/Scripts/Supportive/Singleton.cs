using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supportive
{
	public abstract class Singleton<T> : MonoBehaviour where T : Component
	{
		public static T instance;

		void Awake()
		{
			if (instance == null) instance = this as T;
			else
			{
				Debug.LogError("Singleton instance exists when " + typeof(T).Name + "awake");
			}
		}

	}
}
