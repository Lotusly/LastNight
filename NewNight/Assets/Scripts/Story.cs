using Supportive;
using Ui;
using UnityEngine;
using UnityEngine.UI;


public class Story : Singleton<Story>
{

	private Dialogue.DialogueContaining _tmpDiaCon = new Dialogue.DialogueContaining(); // used to control dialogue
	private int state = 0;
	private string name = "Cara";
	private Option.OptionCon _tmpOptionCon;
	private Option.Cost _tmpCost;
	private TransitionForm.TransitionParameterBlock _transitionParameters;


	// TEMP
	private int sceneIndex = 0;


	private int _num;
	[SerializeField] private TMPStory _tmpStory;
	// TEST
	public Dialogue.DialogueContaining[] _dialogues;
	private string sceneName;

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
		_dialogues=new Dialogue.DialogueContaining[_num];
		for (int i = 0; i < _num; i++)
		{
			_dialogues[i] = _tmpStory.TmpDialogues[i];
		}
		
		
		SwitchDialogue(1);
		//UiManager.instance.CallDialogue();
	}

	public void Transfer()
	{
		transform.position = Coordinate.instance.transform.position;
		
		TransitionForm.instance.ClearParameter(ref _transitionParameters);
		TransitionForm.instance.SetBackgroundParameters(ref _transitionParameters,2,new Vector3(2,0,30), true);
		TransitionForm.instance.SetForegroundParameters(ref _transitionParameters, 2, new Vector3(2,0,13.53f), true );
		UiManager.instance.FadeOutPresentScene(_transitionParameters);
		
		sceneName="scene" + sceneIndex.ToString();
		sceneIndex++;
		
		UiManager.instance.CreateScene(sceneName);
		UiManager.instance.GenerateInScene(sceneName,"Backgrounds/1", new Vector3(-2, 0, 30), true, "Background");
		UiManager.instance.GenerateInScene(sceneName,"Characters/3", new Vector3(-2, 0, 13.53f), true, "Foreground");
		
		TransitionForm.instance.ClearParameter(ref _transitionParameters);
		TransitionForm.instance.SetBackgroundParameters(ref _transitionParameters,2,new Vector3(0,-0.3f,30), true);
		TransitionForm.instance.SetForegroundParameters(ref _transitionParameters, 2, new Vector3(0.75f,-0.3f,13.53f), true );
		UiManager.instance.FadeInScene(sceneName,_transitionParameters);
	}



	public void OnClick( int optionIndex)
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
