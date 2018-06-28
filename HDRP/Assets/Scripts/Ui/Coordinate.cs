using System.Collections;

using UnityEngine;

namespace Ui
{
	public class Coordinate : Supportive.Singleton<Coordinate>
	{


		public Vector3 Space2Screen(Vector3 corSpace)
		{
			float tanFV = Mathf.Tan(Camera.main.fieldOfView * Mathf.PI / 360);
			Vector3 corSpaceRelated = corSpace - Camera.main.transform.position;
			return new Vector3(corSpaceRelated.x * ((float) (Screen.height) / Screen.width) / (tanFV *corSpaceRelated.z),
				corSpaceRelated.y / (corSpaceRelated.z * tanFV), corSpaceRelated.z);
		}

		public Vector3 Screen2Space(Vector3 corScreen)
		{
			float tanFV = Mathf.Tan(Camera.main.fieldOfView * Mathf.PI / 360);
			Vector3 corSpaceRelated = new Vector3(corScreen.x * corScreen.z * tanFV * ((float) (Screen.width) / Screen.height),
				corScreen.y * corScreen.z * tanFV, corScreen.z);
			return Camera.main.transform.position + corSpaceRelated;
		}
	}
}
