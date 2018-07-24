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

			Vector3 v = cam.WorldToViewportPoint(corSpace);

			// Go from unity viewport to lotus viewport
			v.x = v.x * 2 - 1;
			v.y = v.y * 2 - 1;

			return v;
		}

		public Vector3 Screen2Space(Vector3 corScreen, Camera cam = null)
		{
			if (cam == null)
				cam = Camera.main;

			// Go from lotus viewport to unity viewport
			corScreen.x = corScreen.x / 2 + 1/2f;
			corScreen.y = corScreen.y / 2 + 1/2f;

			return cam.ViewportToWorldPoint(corScreen);
		}
	}
}
