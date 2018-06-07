using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{

	public class UiMask : UiItem
	{

		
		// Use this for initialization

		private Quaternion _rotationOutScreen;

		public override void Initialize(Vector3 aimPosition=new Vector3()) // parameter aimPositiom is not used
		{
			_afterArrival.AddListener(UiManager.instance.AfterBackground);
			aimPosition = Coordinate.instance.Space2Screen(transform.position);
			_positionOutScreen = aimPosition;
			_rotationOutScreen = transform.rotation;
			EnableFollowObject();

		}

		public void Reset()
		{
			transform.rotation = _rotationOutScreen;
			SetPosition(_positionOutScreen,true,false,true);
			//transform.position = Coordinate.instance.Screen2Space(_positionOutScreen);
		}

		public void SwitchBackground(Vector2 direction=new Vector2()) // direction here is in screen space
		{
			if (direction.magnitude == 0)
			{
				direction=new Vector2(0,1);
			}
			Vector2 norm = direction.normalized;
			SetPosition(new Vector3(norm.x*5, norm.y*5, _positionOutScreen.z), true, false, true);
			float radius=Mathf.Acos(Vector2.Dot(norm, new Vector2(0, -1)));
			float degree=degree = radius * 180 / Mathf.PI;
			if (direction.x < 0 )
			{
				degree = 360-degree;
			}
			else if (direction.x==0f && direction.y>0)
			{
				degree = 180;
			}

			transform.rotation=Quaternion.Euler(0,0,degree);
			Transfer(new Vector3(0,0,_positionOutScreen.z),
				true,false,true);
			
		}

		
	}
}
