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
			EnableCircleMark();
		}

		void OnMouseExit()
		{
			DisableCircleMark();
		}

		void OnMouseDown()
		{
			UiManager.instance.ZoomIn(this);
			
		}

		public void EnableCircleMark()
		{
			_mark.enabled = true;
			_mark.gameObject.GetComponent<MeshRenderer>().enabled = true;
		}
		
		public void DisableCircleMark()
		{
			_mark.enabled = false;
			_mark.gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
	}
}
