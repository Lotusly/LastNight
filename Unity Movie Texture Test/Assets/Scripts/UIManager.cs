using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

	[SerializeField] private GameObject[] items;
	[SerializeField] private Transform parent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject Generate(int Index, Vector3 Position, bool InScreenSpace)
	{
		if (Index < 0 || Index > items.Length - 1)
		{
			Debug.LogError("Call UIManager to generate UIItem with unproper index");
			return null;
		}

		Vector3 PositionInWorld = Position;
		if (InScreenSpace)
		{
			PositionInWorld = Coordinate.instance.Screen2Space(PositionInWorld);
		}

		GameObject newItem = Instantiate(items[Index],parent);
		newItem.transform.position = PositionInWorld;
		return newItem;

	}
}
