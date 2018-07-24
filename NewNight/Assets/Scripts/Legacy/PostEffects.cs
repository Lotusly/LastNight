// Author: Lele Feng
// Reference: Unity Shader 入门精要

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supportive
{
	[ExecuteInEditMode]

	public class PostEffects : MonoBehaviour
	{

		public static PostEffects instance;
		public void CheckResources()
		{
			bool isSupported = CheckSupport();

			if (!isSupported)
			{
				NotSupported();
			}
		}

		public bool CheckSupport()
		{
			if (SystemInfo.supportsImageEffects == false || SystemInfo.supportsRenderTextures == false)
			{
				Debug.LogWarning("This platform does not support image effects or render textures");
				return false;
			}

			return true;
		}

		public void NotSupported()
		{
			enabled = false;
		}

		void Awake()
		{
			if (instance == null) instance = this;
		}

		public Material CheckShaderAndCreateMaterial(Shader shader, Material material)
		{
			if (shader == null)
			{
				return null;
			}

			if (shader.isSupported && material && material.shader == shader)
			{
				return material;
			}

			if (!shader.isSupported)
			{
				return null;
			}
			else
			{
				material = new Material(shader);
				material.hideFlags = HideFlags.DontSave;

				if (material)
				{
					return material;
				}
				else
				{
					return null;
				}
			}
		}
	}
}
