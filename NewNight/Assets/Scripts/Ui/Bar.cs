
using System.Runtime.Serialization.Formatters;
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
			if (_workingScale == null) _workingScale = transform;
		}
		
		public void SetValue(int newValue)
		{
			if (newValue < 0)
			{
				return;
			}
			if (newValue > _maxValue)
			{
				_value = _maxValue;
				_workingScale.localScale=new Vector3(1,1,1);
			}
			else
			{
				_value = newValue;
				_workingScale.localScale=new Vector3((float)newValue/_maxValue,1,1);
			}
			
		}

		public bool ChechModify(int addition)
		{
			if (_value + addition < 0) return false;
			else return true;
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
