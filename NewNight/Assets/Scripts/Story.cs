using Supportive;
using Ui;
using UnityEngine;
using UnityEngine.UI;

public class Story : Singleton<Story>
{
	private int state = 0;
	private int _num;
	private string name = "Cara";
	private Option.Cost _tmpCost;
	private Option.OptionCon _tmpOptionCon;
	private TransitionForm.TransitionParameterBlock _transitionParameters;
	private Dialogue.DialogueContaining _tmpDiaCon = new Dialogue.DialogueContaining(); // used to control dialogue

	[SerializeField] private TMPStory _tmpStory;

	// TEMP
	private int sceneIndex = 0;

	// TEST
	public Dialogue.DialogueContaining[] _dialogues;
	private string sceneName;

	// Establish the scene and dialogue (Aaron)
	public void Initialize()
	{
		//UiManager.instance.PutPotentialProp(0,new Vector3(0.3f,-0.2f,15f));

		sceneName = "scene" + sceneIndex.ToString();
		sceneIndex++;

		UiManager.instance.CreateScene(sceneName);
		UiManager.instance.SwitchScene(sceneName);
		UiManager.instance.GenerateInPresentScene("Backgrounds/0", new Vector3(0, 0, 30), false,"Background");
		UiManager.instance.GenerateInPresentScene("Characters/2", new Vector3(-5.2f, -3.6f, 13.53f), false,"Foreground");
		_transitionParameters = new TransitionForm.TransitionParameterBlock();

		// Clone from TmpStory to _dialogues (local variable)
		_num = _tmpStory.TmpDialogues.Length;
		_dialogues = new Dialogue.DialogueContaining[_num];
		for (int i = 0; i < _num; i++)
		{
			_dialogues[i] = _tmpStory.TmpDialogues[i];
		}

		SwitchDialogue(1);
		//UiManager.instance.CallDialogue();
	}

	



	public void OnClick(int optionIndex)
	{
		// TEST
		print("click "+optionIndex.ToString()+" when "+state.ToString());
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
			//UiManager.instance.ZoomOut();
			state = 0;
		}

	}
}
