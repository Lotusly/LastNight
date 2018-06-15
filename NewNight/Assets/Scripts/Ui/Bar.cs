using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Bar : MonoBehaviour
	{
		private float _value;
		public bool SetValue(float newValue)
		{
			if (newValue > 1 || newValue < 0)
			{
				Debug.LogError("Bar: SetValue use invalid value: "+newValue.ToString());
				return false;
			}
			else
			{
				_value = newValue;
				transform.localScale=new Vector3(newValue,transform.localScale.y,transform.localScale.z);
				return true;
			}
			
		}

		public float GetValue()
		{
			return _value;
		}

		
	}
}
