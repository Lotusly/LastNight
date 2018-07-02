using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class StatBar : UiItem
	{
		
		private Dictionary<string, Bar> _stats = new Dictionary<string, Bar>();

		public override void  Initialize(Vector3 aimPosition=new Vector3())
		{
			SetPosition(_positionOutScreen,true,false, true );
			Bar[] bars = GetComponentsInChildren<Bar>();
			for (int i = 0; i < bars.Length; i++)
			{
				_stats.Add(bars[i].name,bars[i]);
			}
		}

		public void SetStat(string name, int value)
		{
			if (!_stats.ContainsKey(name))
			{
				Debug.LogError("StatBar: SetState tries to set unexisted stat "+name+" to "+value.ToString());
			}
			else
			{
				_stats[name].SetValue(value);
			}
		}
		
		public void ModifyStat(string name, int value)
		{
			if (!_stats.ContainsKey(name))
			{
				Debug.LogError("StatBar: ModifyStat tries to modify unexisted stat "+name+" "+value.ToString());
			}
			else
			{
				_stats[name].SetValue(_stats[name].GetValue()+value);
			}
		}

		public bool CheckStat(string name, int addition)
		{
			if (!_stats.ContainsKey(name))
			{
				Debug.LogError("StatBar: CheckStat tries to check unexisted stat "+name+" "+addition.ToString());
				return false;
			}
			else
			{
				return _stats[name].ChechModify(addition);
			}
		}
		
	}
}
