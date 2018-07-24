using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(TextureBlendingRenderer), PostProcessEvent.AfterStack, "Custom/TextureBlending")]
public sealed class TextureBlending : PostProcessEffectSettings
{
	[Range(0f, 3f), Tooltip("Grayscale effect intensity.")]
	public FloatParameter blend = new FloatParameter {value = 1f};

	[Tooltip("Texture to mix.")]
	public TextureParameter tex = new TextureParameter{value=null};
	//public TextureParameter tex;
}

public sealed class TextureBlendingRenderer : PostProcessEffectRenderer<TextureBlending>
{
	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/TextureBlending"));
		sheet.properties.SetFloat("_Blend", settings.blend);
		sheet.properties.SetTexture("_TextureToMix",settings.tex.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
}
