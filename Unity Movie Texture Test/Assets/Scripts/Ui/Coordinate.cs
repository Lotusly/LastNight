using System.Collections;

using UnityEngine;

namespace Ui
{
	public class Coordinate : Supportive.Singleton<Coordinate>
	{


		public Vector3 Space2Screen(Vector3 corSpace)
		{
			float tanFV = Mathf.Tan(Camera.main.fieldOfView * Mathf.PI / 360);
			return new Vector3(corSpace.x * ((float) (Screen.height) / Screen.width) / (tanFV * corSpace.z),
				corSpace.y / (corSpace.z * tanFV), corSpace.z);
		}

		public Vector3 Screen2Space(Vector3 corScreen)
		{
			float tanFV = Mathf.Tan(Camera.main.fieldOfView * Mathf.PI / 360);
			return new Vector3(corScreen.x * corScreen.z * tanFV * ((float) (Screen.width) / Screen.height),
				corScreen.y * corScreen.z * tanFV, corScreen.z);
		}
	}
}
