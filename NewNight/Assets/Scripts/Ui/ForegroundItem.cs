using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class ForegroundItem : UiItem
	{

		private Vector3 _potentialPosition; // this is always in screen space
		public override void Initialize(Vector3 aimPosition=new Vector3())
		{
			SetPosition(transform.position, false, false, true);
			UpdateOriginPosition();
			if (aimPosition != Vector3.zero)
			{
				_potentialPosition = aimPosition;
			}
			else
			{
				Debug.LogWarning("ForegroundItem didn't get potential position assigned when initialize");
				aimPosition = transform.position;
			}
		}

		public void SetPotentialPosition(Vector3 position)
		{
			_potentialPosition = position;
		}

		public override void MoveBack()
		{
			Transfer(GetOriginalPosition(),false,false,true);
		}

		public override void MoveOut(UiItem focus=null)
		{
			Transfer(_potentialPosition,true,true,true);
		}
	}
}
