
using System.Collections.Generic;
using Ui;
using UnityEngine;

namespace Supportive
{
	public class Test : MonoBehaviour
	{
		[SerializeField] private Vector3 _spaceInScreen;
		[SerializeField] private List<GameObject> _objects;
		[SerializeField] private int _index;

		[SerializeField] private bool _generate = false;
		[SerializeField] private bool _eliminate = false;
		[SerializeField] private bool _transfer = false;


		void Update()
		{
			if (_generate)
			{
				_generate = false;
				GameObject newItem = Ui.UiManager.instance.Generate(0, _spaceInScreen, true);
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
		}
	}
}
