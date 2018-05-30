using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Character : UiItem
	{
		private MarkCircle _mark;

		
		
		public override void Initialize(Vector3 aimPosition=new Vector3())
		{
			_mark = GetComponentInChildren<MarkCircle>();
			UpdateOriginPosition();
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

		public override void MoveOut(UiItem focus = null)
		{
			if(focus==this)
				Transfer(new Vector3(-0.5f,-0.4f,5),true,true);
		}

		
	}
}
