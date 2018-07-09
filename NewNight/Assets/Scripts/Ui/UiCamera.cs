using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	[RequireComponent (typeof(Camera))]
	public class UiCamera : UiItem
	{
		public Material FishEyeMaterial;
		public Material briSatConMaterial;

		public override void MoveBack()
		{
			Transfer(Vector3.zero,false,false);
		}
		
		public override void Initialize(Vector3 aimPosition=new Vector3())
		{
			SetPosition(Vector3.zero,false,true);
			UpdateOriginPosition();
		}
		
		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (FishEyeMaterial != null)
			{

				Graphics.Blit(src, dest, FishEyeMaterial);
			}
			else
			{
				Graphics.Blit(src,dest);
			}
			
			if (briSatConMaterial != null)
			{

				Graphics.Blit(src, dest, briSatConMaterial);
			}
			else
			{
				Graphics.Blit(src,dest);
			}
		}

		
	}
}
