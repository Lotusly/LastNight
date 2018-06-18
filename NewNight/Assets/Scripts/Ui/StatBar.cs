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
			SetPosition(new Vector3(1,-1,8),true,false, true );
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
		
	}
}
