using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Ui;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FadeOutIn : ScriptableObject {

	public delegate IEnumerator Movement(Transform tran, Vector3 newPosition, bool inScreen, float speed); // speed here means finish the whole movement in 1/speed seconds
	public static Movement[] Movements;

	void Awake()
	{
		Movements = new Movement[6];
		Movements[0] = Nothing;
		Movements[1] = Direct;
		Movements[2] = Average;
	}
	
	public static IEnumerator Nothing(Transform tran, Vector3 newPosition, bool inScreen, float speed=1)
	{
		Debug.Log("Nothing");
		yield return null;
	}

	public static IEnumerator Direct(Transform tran, Vector3 newPosition, bool inScreen, float speed=1)
	{
		Debug.Log("Direct");
		yield return null;
		if (inScreen) tran.position = Coordinate.instance.Screen2Space(newPosition);
		else tran.position = newPosition;
	}

	public static IEnumerator Average(Transform tran, Vector3 newPosition, bool inScreen, float speed=1)
	{
		Debug.Log("Average");
		yield return null;
		Vector3 oriPosition;
		if (inScreen) oriPosition = Coordinate.instance.Space2Screen(tran.position);
		else oriPosition = tran.position;
		if (speed > 0)
		{
			float tMax = 1 / speed;
			Vector3 perSecond = (newPosition - oriPosition) *speed;
			float t = 0;
			Vector3 presentPosition=oriPosition;
			while (t < tMax)
			{
				if (inScreen)
				{
					presentPosition += perSecond * Time.deltaTime;
					tran.position = Coordinate.instance.Screen2Space(presentPosition);
				}
				else
				{
					tran.position += perSecond * Time.deltaTime;
				}

				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
	}
	
	
}
