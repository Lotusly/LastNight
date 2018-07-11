using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Ui
{
	[RequireComponent (typeof(Camera))]
	public class UiCamera : UiItem
	{

		public PostProcessVolume Volume;
		public override void MoveBack()
		{
			Transfer(Vector3.zero,false,false);
		}
		
		public override void Initialize(Vector3 aimPosition=new Vector3())
		{
			SetPosition(Vector3.zero,false,true);
			UpdateOriginPosition();
		}
		


		
	}
}
