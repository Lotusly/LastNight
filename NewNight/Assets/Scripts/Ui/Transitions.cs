using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Ui
{

	public class Transitions : ScriptableObject
	{
		public struct TransitionParameterBlock
		{
			public int BackgroundMethod;
			public Vector3 BackgroundPosition;
			public bool BackgroundInScreen;
			public float BackgroundSpeed;
			
			public int MidgroundMethod;
			public Vector3 MidgroundPosition;
			public bool MidgroundInScreen;
			public float MidgroundSpeed;
			
			public int ForegroundMethod;
			public Vector3 ForegroundPosition;
			public bool ForegroundInScreen;
			public float ForegroundSpeed;
			
			public int BackgroundCameraMethod;
			public Vector3 BackgroundCameraPosition;
			public float BackgroundCameraSpeed;
			
			public int MainCameraMethod;
			public Vector3 MainCameraPosition;
			public float MainCameraSpeed;
		};

		void ClearParameter(ref TransitionParameterBlock block)
		{
			block.BackgroundMethod = 0;
			block.MidgroundMethod = 0;
			block.ForegroundMethod = 0;
			block.BackgroundCameraMethod = 0;
			block.MainCameraMethod = 0;

			block.BackgroundPosition = Vector3.zero;
			block.MidgroundPosition = Vector3.zero;
			block.ForegroundPosition = Vector3.zero;
			block.BackgroundCameraPosition = Vector3.zero;
			block.MainCameraPosition = Vector3.zero;

			block.BackgroundInScreen = false;
			block.MidgroundInScreen = false;
			block.ForegroundInScreen = false;

			block.BackgroundSpeed = 1;
			block.MidgroundSpeed = 1;
			block.ForegroundSpeed = 1;
			block.BackgroundCameraSpeed = 1;
			block.MainCameraSpeed = 1;
		}

		void SetBackgroundParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1)
		{
			block.BackgroundMethod = method;
			block.BackgroundPosition = newPosition;
			block.BackgroundInScreen = inScreen;
			block.BackgroundSpeed = speed;
		}
		
		void SetMidgroundParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1)
		{
			block.MidgroundMethod = method;
			block.MidgroundPosition = newPosition;
			block.MidgroundInScreen = inScreen;
			block.MidgroundSpeed = speed;
		}
		
		void SetForegroundParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1)
		{
			block.ForegroundMethod = method;
			block.ForegroundPosition = newPosition;
			block.ForegroundInScreen = inScreen;
			block.ForegroundSpeed = speed;
		}
		
		void SetBackgroundCameraParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1)
		{
			block.BackgroundCameraMethod = method;
			block.BackgroundCameraPosition = newPosition;
			block.BackgroundCameraSpeed = speed;
		}
		
		void SetMainCameraParameters(ref TransitionParameterBlock block, int method, Vector3 newPosition, bool inScreen, float speed=1)
		{
			block.MainCameraMethod = method;
			block.MainCameraPosition = newPosition;
			block.MainCameraSpeed = speed;
		}
		
	}
}
