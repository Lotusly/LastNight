using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Bar : MonoBehaviour
	{
		[SerializeField]private int _value=0;
		[SerializeField]private int _maxValue=100; 
		public bool SetValue(int newValue)
		{
			if (newValue > _maxValue || newValue < 0)
			{
				Debug.LogError("Bar: "+name+" SetValue use invalid value: "+newValue.ToString());
				return false;
			}
			else
			{
				_value = newValue;
				transform.localScale=new Vector3((float)newValue/_maxValue,transform.localScale.y,transform.localScale.z);
				return true;
			}
			
		}

		public int GetValue()
		{
			return _value;
		}

		
	}
}
