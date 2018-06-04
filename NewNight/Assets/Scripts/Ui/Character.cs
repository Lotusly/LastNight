using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Character : UiItem
	{
		private MarkCircle _mark;

		[SerializeField] private Vector3 _moveOutTo;

		
		
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
				Transfer(_moveOutTo,true,true);
		}

		
	}
}
