using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLeaking : PostEffects
{

	public Shader briSatConShader;
	private Material briSatConMaterial;

	public Material material
	{
		get
		{
			briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader, briSatConMaterial);
			return briSatConMaterial;
		}
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (material != null)
		{

			Graphics.Blit(src, dest, material);
		}
		else
		{
			Graphics.Blit(src,dest);
		}
	}
}
