using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{

	public class UiMask : UiItem
	{

		
		// Use this for initialization

		private Quaternion _rotationOutScreen;
		private Background[] _backgrounds;
		private Vector2 _originalInterval;

		public override void Initialize(Vector3 aimPosition=new Vector3()) // parameter aimPositiom is not used
		{
			UpdateState(ref presentState,true,true);
			_backgrounds = GetComponentsInChildren<Background>();
			_originalInterval = new Vector2(
				_backgrounds[1].transform.localPosition.y - _backgrounds[0].transform.localPosition.y,
				_backgrounds[2].transform.localPosition.y - _backgrounds[1].transform.localPosition.y);
			AfterArrival.AddListener(UiManager.instance.AfterBackground);
			aimPosition = Coordinate.instance.Space2Screen(transform.position);
			_positionOutScreen = aimPosition;
			_rotationOutScreen = transform.rotation;


		}

		public void Reset()
		{
			transform.rotation = _rotationOutScreen;
			SetPosition(_positionOutScreen,true,true);
		}

		public void SetInterval(Vector2 interval)
		{
			if(interval[0]<0) SetInterval(_originalInterval); // interval[0]<0 => reset interval
			else
			{
				_backgrounds[1].transform.localPosition = _backgrounds[0].transform.localPosition + Vector3.up * interval[0];
				_backgrounds[2].transform.localPosition = _backgrounds[1].transform.localPosition + Vector3.up * interval[1];
			}
		}

		public void SwitchBackground(Vector2 direction=new Vector2()) // direction here is in screen space
		{
			if (direction.magnitude == 0)
			{
				direction=new Vector2(0,1);
			}
			Vector2 norm = direction.normalized;
			SetPosition(new Vector3(norm.x*4.2f, norm.y*4.2f, _positionOutScreen.z), true, true); // this can be upgraded to calculate the edge distance
			float radius=Mathf.Acos(Vector2.Dot(norm, new Vector2(0, -1)));
			float degree = radius * 180 / Mathf.PI;
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
				true,true,1,0.9f);
			
		}

		
	}
}
