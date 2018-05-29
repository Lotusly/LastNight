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
		GameObject woman = UiManager.instance.Generate("Characters", 0, new Vector3(-1.4f, -0.2f, 15), true);
		woman.GetComponent<Character>().Transfer(new Vector3(0,-0.2f,15),true,true );
	}
}
