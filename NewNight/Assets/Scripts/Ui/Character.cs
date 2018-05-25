using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Character : UiItem
	{
		private MarkCircle _mark;

		
		// Use this for initialization
		void Start()
		{
			_mark = GetComponentInChildren<MarkCircle>();
		}

		// Update is called once per frame
		void Update()
		{

		}

		void OnMouseEnter()
		{
			_mark.Pop();
		}

		void OnMouseExit()
		{
			_mark.Shrink();
		}

		void OnMouseDown()
		{
			UiManager.instance.ZoomIn(this);
			
		}

		
	}
}
