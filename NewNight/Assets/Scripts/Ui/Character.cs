using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Character : UiItem
	{
		private MarkCircle _mark;
		private BoxCollider _col;

		[SerializeField] private Vector3 _moveOutTo;

		
		
		public override void Initialize(Vector3 aimPosition=new Vector3())
		{
			_col=GetComponent<BoxCollider>();
			_mark = GetComponentInChildren<MarkCircle>();
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
			_col.enabled = false;
			if(focus==this)
				Transfer(_moveOutTo,true);
			
		}


		public override void MoveBack()
		{
			_col.enabled = true;
			Transfer(lastState.Position,lastState.InScreen);
		}

		
		
	}
}
