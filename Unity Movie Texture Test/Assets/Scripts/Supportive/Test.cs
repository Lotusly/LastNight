
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour
{
	[SerializeField] private Vector3 spaceInScreen;
	[SerializeField] private List<GameObject> objects;
	[SerializeField] private int index;
	
	[SerializeField] private bool generate=false;
	[SerializeField] private bool eliminate = false;


	void Update () {
		if (generate)
		{
			generate = false;
			GameObject newItem = UIManager.instance.Generate(0, spaceInScreen, true);
			objects.Add(newItem);
		}

		if (eliminate)
		{
			eliminate = false;
			Destroy(objects[index]);
			objects.RemoveAt(index);
			
		}
	}
}
