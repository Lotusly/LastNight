using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FadeOut : ScriptableObject {

	public static IEnumerator MoveOut()
	{
		Debug.Log("move out successfully.");
		yield return null;
	}
}
