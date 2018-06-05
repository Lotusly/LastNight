using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{

	public class UiMask : UiItem
	{

		
		// Use this for initialization


		public override void Initialize(Vector3 aimPosition=new Vector3()) // parameter aimPositiom is not used
		{
			aimPosition = Coordinate.instance.Space2Screen(transform.position);
			_positionOutScreen = aimPosition;
			_followingCamera = true;
		}

		public void SwitchBackground()
		{
			transform.position = Coordinate.instance.Screen2Space(_positionOutScreen);
			Transfer(new Vector3(0,0,_positionOutScreen.z),
				true,false,true);
			
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
