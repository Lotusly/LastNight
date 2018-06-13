using System.Collections;
using System.Collections.Generic;
using Supportive;
using Ui;
using UnityEngine;

public class Story : Singleton<Story>
{

	private Dialogue.DialogueContaining _tmpDiaCon = new Dialogue.DialogueContaining(); // used to control dialogue
	private int state = 0;
	private string name = "Cara";
	private char[] _vowels = new char[5] {'a', 'e', 'i', 'o', 'u'};

	private char[] _consonant = new char[21]
		{'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'};

	private int[,] _events = new int[6,5]
	{
		{
			-1, 1, 2, -1, -1
		},
		{
			3,-1,-1,-1,-1
		},
		{
			-1,0,3,4,-1
		},
		{
			5,-1,-1,-1,-1
		},
		{
			5,-1,-1,-1,-1
		},
		{
			0,-1,-1,-1,-1
		}

	};
	

	public void Initialize()
	{
		//UiManager.instance.Generate("Backgrounds", 1, new Vector3(0, 0, 30), false);
		UiManager.instance.PutPotentialProp(0,new Vector3(0.3f,-0.2f,15f));
		Character woman = (Character)UiManager.instance.Generate("Characters", 1, new Vector3(-1.8f, -0.1f, 15), true);
		woman.Transfer(new Vector3(0,-0.1f,15),true,false);
		woman.SetOriginPosition(new Vector3(0,-0.1f,15),true);
		
		_tmpDiaCon.Options=new List<string>();
		SwitchDialogue(0);
		
		
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
		print("click "+optionIndex.ToString()+" when "+state.ToString());
		SwitchDialogue(_events[state,optionIndex]);
	}
	
	// TEMP
	private void SwitchDialogue(int i)
	{
		
		switch (i)
		{
			case 0:
			{
				_tmpDiaCon.Name = name;
				_tmpDiaCon.Containing = "Hello! My name is <color=white>"+name+"</color>! Shall we DANCE ?";
				_tmpDiaCon.Options.Clear();
				_tmpDiaCon.Options.Add("\"Of course! Let's Dance!\"");
				_tmpDiaCon.Options.Add("Ignore her and dance alone.");
				UiManager.instance.SetDialogueCon(_tmpDiaCon);
				state = i;
				break;
			}
			case 1:
			{
				_tmpDiaCon.Name = name;
				_tmpDiaCon.Containing = "Stupid! I was just joking. I don't want to dance with you!";
				_tmpDiaCon.Options.Clear();
				UiManager.instance.SetDialogueCon(_tmpDiaCon);
				state = i;
				break;
			}
			case 2:
			{
				_tmpDiaCon.Name = name;
				_tmpDiaCon.Containing = "Oh! Why don't you dance with me? Don't you want to consider it again?";
				_tmpDiaCon.Options.Clear();
				_tmpDiaCon.Options.Add("Apologize to her.");
				_tmpDiaCon.Options.Add("Ignore her. You are the best! She can only drag you back!");
				_tmpDiaCon.Options.Add("Hit her. She is too noisy.");
				UiManager.instance.SetDialogueCon(_tmpDiaCon);
				state = i;
				break;
			}
			case 3:
			{
				_tmpDiaCon.Name = "";
				_tmpDiaCon.Containing = name+ " quickly steps away, leaving you alone.";
				_tmpDiaCon.Options.Clear();
				UiManager.instance.SetDialogueCon(_tmpDiaCon);
				SwitchName();
				state = i;
				break;
			}
			
			case 4:
			{
				_tmpDiaCon.Name = "";
				_tmpDiaCon.Containing = name+" was knocked down. It looks she will not wake up for a while.";
				_tmpDiaCon.Options.Clear();
				UiManager.instance.SetDialogueCon(_tmpDiaCon);
				state = i;
				SwitchName();
				break;
			}
			case 5:
			{
				UiManager.instance.ClearDialogue();
				UiManager.instance.ZoomOut();
				state = i;
				break;
			}
		}
	}
}
