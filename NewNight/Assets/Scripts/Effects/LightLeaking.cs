using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLeaking : Supportive.PostEffects
{
	public Material briSatConMaterial;

	/*public Material material
	{
		get
		{
			briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader, briSatConMaterial);
			return briSatConMaterial;
		}
	}*/

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
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
