using System.Collections;
using System.Collections.Generic;
using Supportive;
using Ui;
using UnityEngine;

public class Story : Singleton<Story>
{
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Initialize()
	{
		//UiManager.instance.Generate("Backgrounds", 1, new Vector3(0, 0, 30), false);
		UiItem woman = UiManager.instance.Generate("Characters", 1, new Vector3(-1.8f, -0.1f, 15), true);
		woman.GetComponent<Character>().Transfer(new Vector3(0,-0.1f,15),true,true );
		//UiManager.instance.GenerateForegroundItem(0, new Vector3(1.2f, 1.2f, 4), new Vector3(0.8f, 0.7f, 4), true);
	}
}
