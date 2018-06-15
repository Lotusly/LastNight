using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
	public class Dialogue : ForegroundItem
	{
		[Serializable]public struct DialogueContaining
		{
			public int IndexToOutOption;
			public string Name;
			public string Containing;
			public List<Option.OptionCon> Options;
			//public Option.OptionCon[] Options;
		};

		[SerializeField] private TextMeshPro _name;
		[SerializeField] private TextMeshPro _containing;
		[SerializeField] private Option[] _options; // can optimize: initialize _options
		private DialogueContaining _diaCon;


		public void SetDialogueContaining(DialogueContaining newDiaCon, bool updateNow=true)
		{
			_diaCon = newDiaCon;
			if(updateNow) UpdateDialogueContaining();
			
		}

		public void UpdateDialogueContaining()
		{
			_name.text = _diaCon.Name;
			_containing.text = _diaCon.Containing;
			for (int i = 0; i < _options.Length; i++)
			{
				if (i < _diaCon.Options.Count)
				{
					_options[i].SetOption(_diaCon.Options[i]);
				}
				else
				{
					_options[i].ClearOption();
				}
			}
		}

		public void ClearDialogue()
		{
			_name.text = "";
			_containing.text = "";
			for (int i = 0; i < _options.Length; i++)
			{
				_options[i].ClearOption();
			}
		}


	
}
}
