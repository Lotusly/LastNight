using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Supportive;
using Ui;
using UnityEngine;

public class Story : Singleton<Story>
{

	private Dialogue.DialogueContaining _tmpDiaCon = new Dialogue.DialogueContaining(); // used to control dialogue
	private int state = 0;
	private string name = "Cara";
	private char[] _vowels = new char[5] {'a', 'e', 'i', 'o', 'u'};
	private Option.OptionCon _tmpOptionCon;
	private Option.Cost _tmpCost;

	private char[] _consonant = new char[21]
		{'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'};



	private int _num;
	[SerializeField] private TMPStory _tmpStory;
	// TEST
	public Dialogue.DialogueContaining[] _dialogues;
	

	public void Initialize()
	{
		//UiManager.instance.Generate("Backgrounds", 1, new Vector3(0, 0, 30), false);
		UiManager.instance.PutPotentialProp(0,new Vector3(0.3f,-0.2f,15f));
		Character woman = (Character)UiManager.instance.Generate("Characters", 1, new Vector3(-1.8f, -0.1f, 15), true);
		woman.Transfer(new Vector3(0,-0.1f,15),true,false);
		woman.SetOriginPosition(new Vector3(0,-0.1f,15),true);
		
		// Clone from TmpStory to _dialogues (local variable)
		_num = _tmpStory.TmpDialogues.Length;
		_dialogues=new Dialogue.DialogueContaining[_num];
		for (int i = 0; i < _num; i++)
		{
			_dialogues[i] = _tmpStory.TmpDialogues[i];
		}
		
		//_tmpDiaCon.Options=new List<Option.OptionCon>();
		//_tmpOptionCon.Costs = new List<Option.Cost>();
		SwitchDialogue(1);
		
		
		//UiManager.instance.GenerateForegroundItem(0, new Vector3(1.2f, 1.2f, 4), new Vector3(0.8f, 0.7f, 4), true);
	}

	private void SwitchName()
	{

		name = _consonant[Random.Range(0, 21)].ToString().ToUpper() + _vowels[Random.Range(0, 5)] +
		       _consonant[Random.Range(0, 21)] + _vowels[Random.Range(0, 5)];


	}

	public void OnClick( int optionIndex)
	{
		// TEST
		//print("click "+optionIndex.ToString()+" when "+state.ToString());
		if (state > 0)
		{
			if (optionIndex > 0) SwitchDialogue(_dialogues[state - 1].Options[optionIndex - 1].IndexToInOption);
			else
				SwitchDialogue(_dialogues[state - 1].IndexToOutOption);
		}
		// TMP
		else
		{
			if (optionIndex == 0)
			{
				SwitchDialogue(1);
			}
		}
	}
	
	// TEMP
	private void SwitchDialogue(int i)
	{
		if (i > 0)
		{
			UiManager.instance.SetDialogueCon(_dialogues[i-1]);
			state = i;
		}
		if(i==0)
		{
			UiManager.instance.ClearDialogue();
			UiManager.instance.ZoomOut();
			state = 0;
		}
		
	}
}
