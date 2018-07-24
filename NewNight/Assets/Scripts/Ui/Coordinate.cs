using System.Collections;

using UnityEngine;

namespace Ui
{
	public class Coordinate : Supportive.Singleton<Coordinate>
	{
		public Vector3 Space2Screen(Vector3 corSpace, Camera cam = null)
		{
			if (cam == null)
				cam = Camera.main;

			return cam.WorldToScreenPoint(corSpace);
		}

		public Vector3 Screen2Space(Vector3 corScreen, Camera cam = null)
		{
			if (cam == null)
				cam = Camera.main;

			return cam.ScreenToWorldPoint(corScreen);
		}
	}
}
