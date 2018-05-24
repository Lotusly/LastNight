
using System.Collections.Generic;
using Ui;
using UnityEngine;

namespace Supportive
{
	public class Test : MonoBehaviour
	{
		[SerializeField] private string pathName;
		[SerializeField] private int _index;
		[SerializeField] private Vector3 _spaceInScreen;
		[SerializeField] private List<GameObject> _objects;
		

		[SerializeField] private bool _generate = false;
		[SerializeField] private bool _eliminate = false;
		[SerializeField] private bool _transfer = false;
		[SerializeField] private bool _clearAll = false;


		void Update()
		{
			if (_generate)
			{
				_generate = false;
				GameObject newItem = Ui.UiManager.instance.Generate(pathName,_index, _spaceInScreen, true);
				_objects.Add(newItem);
				
			}

			if (_eliminate)
			{
				_eliminate = false;
				Destroy(_objects[_index]);
				_objects.RemoveAt(_index);

			}

			if (_transfer)
			{
				_transfer = false;
				_objects[_index].GetComponent<UiItem>().Transfer(Coordinate.instance.Screen2Space(_spaceInScreen));
			}

			if (_clearAll)
			{
				_clearAll = false;
				for (int i = 0; i < _objects.Count; i++)
				{
					Destroy(_objects[i]);
				}
				_objects.Clear();
			}
		}
	}
}
