using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{

	public class UiManager : Supportive.Singleton<UiManager>
	{

		[SerializeField] private GameObject[] _items;
		[SerializeField] private Transform _parent;


		public GameObject Generate(int index, Vector3 position, bool inScreenSpace)
		{
			if (index < 0 || index > _items.Length - 1)
			{
				Debug.LogError("Call UiManager to generate UiItem with unproper index");
				return null;
			}

			Vector3 positionInWorld = position;
			if (inScreenSpace)
			{
				positionInWorld = Coordinate.instance.Screen2Space(positionInWorld);
			}

			GameObject newItem = Instantiate(_items[index], _parent);
			newItem.transform.position = positionInWorld;
			return newItem;

		}
	}
}
