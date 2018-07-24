using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEye : Supportive.PostEffects
{
	public Material FishEyeMaterial;

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
	}
}
