
using TMPro;
using UnityEngine;


namespace Ui
{
	public class Bar : MonoBehaviour
	{
		[SerializeField]private int _value=0;
		[SerializeField]private int _maxValue=100;
		[SerializeField] private Transform _workingScale;
		private TextMeshPro _text;
		

		public void Awake()
		{
			_text = GetComponentInChildren<TextMeshPro>();
		}
		
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
				_workingScale.localScale=new Vector3((float)newValue/_maxValue,1,1);
				return true;
			}
			
		}

		public int GetValue()
		{
			return _value;
		}

		void OnMouseEnter()
		{
			_text.text = _value.ToString() + "/" + _maxValue.ToString();
		}

		void OnMouseExit()
		{
			_text.text = "";
		}

		
	}
}
