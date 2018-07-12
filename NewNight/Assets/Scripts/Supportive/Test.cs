
using System.Collections.Generic;
using Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Supportive
{
	public class Test : Singleton<Test>
	{


		public int index=1;
		
		private Image [] _images;




		void Start()
		{
			_images = GetComponentsInChildren<Image>();
		}

		public void ShowImage(int index)
		{
			_images[index].enabled = true;
		}
		
		void Update()
		{

			
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				UiManager.instance.ZoomOut();
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				index = (index + 1) % 2;
				UiManager.instance.SwitchBackground(index);
			}

			if (Input.GetMouseButtonDown(0))
			{
				//UiManager.instance.FadeOutBackground("");
			}

			
			/*if (Input.GetMouseButtonDown(0))
			{
				for (int i = 0; i < 10; i++)
				{
					UiManager.instance.Generate("Midground", 0, new Vector3(-1f+i*0.2f,0,6), true, false);

				}
				Vector2 direction = new Vector2(2*Input.mousePosition.x/Screen.width-1,2*Input.mousePosition.y/Screen.height-1);
				//print(direction);
				UiManager.instance.SwitchBackground(index,direction);
			}*/

		
			
			// the block below is to test Coordinate class and UiManager.Generate and UiItem.Transfer
			/*
			 
			[SerializeField] private string pathName;
			[SerializeField] private int _index;
			[SerializeField] private Vector3 _spaceInScreen;
			[SerializeField] private List<GameObject> _objects;
			
	
			[SerializeField] private bool _generate = false;
			[SerializeField] private bool _eliminate = false;
			[SerializeField] private bool _transfer = false;
			[SerializeField] private bool _clearAll = false;
			
			if (_generate)
			{
				_generate = false;
				GameObject newItem = Ui.UiManager.instance.Generate(pathName,_index, _spaceInScreen, true).gameObject;
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
				_objects[_index].GetComponent<UiItem>().Transfer(Coordinate.instance.Screen2Space(_spaceInScreen),true,true);
			}

			if (_clearAll)
			{
				_clearAll = false;
				for (int i = 0; i < _objects.Count; i++)
				{
					Destroy(_objects[i]);
				}
				_objects.Clear();
			}*/
		}
	}
}
