
// Incorporates HSBC effects and gradient overlay blending for sprites.

Shader "BlendModes/Extra/SpriteGradientHSBC" 
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OverlayTex("Overlay Texture", 2D) = "white" {}
        _MainTint("Main Tint", Color) = (1,1,1,1)
        _OverlayTint("Overlay Tint", Color) = (1,1,1,1)
        _OpacityRamp ("Overlay Opacity Ramp", Range(0.0, 1.0)) = 0.5
        _Hue("Hue", Range(0.0, 1.0)) = 0
        _Saturation("Saturation", Range(-1.0, 1.0)) = 0
        _Brightness("Brightness", Range(-1.0, 1.0)) = 0
        _Contrast("Contrast", Range(-1.0, 1.0)) = 0
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			Name "BlendEffect"

			Stencil
            {
                Ref [_StencilRef]
                Comp [_BlendStencilComp]
            }

			CGPROGRAM
			
			#pragma target 3.0			
			#pragma multi_compile BmDarken BmMultiply BmColorBurn BmLinearBurn BmDarkerColor BmLighten BmScreen BmColorDodge BmLinearDodge BmLighterColor BmOverlay BmSoftLight BmHardLight BmVividLight BmLinearLight BmPinLight BmHardMix BmDifference BmExclusion BmSubtract BmDivide BmHue BmSaturation BmColor BmLuminosity
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#pragma vertex ComputeVertex
			#pragma fragment ComputeFragment

            #include "UnityCG.cginc"	
            #include "../BlendModes.cginc"

	        sampler2D _MainTex, _OverlayTex;
            float4 _OverlayTex_ST;
	        fixed4 _MainTint, _OverlayTint;
            float _OpacityRamp;
            fixed _Hue, _Saturation, _Brightness, _Contrast;
			
	        struct VertexInput
	        {
		        float4 Vertex : POSITION;
		        fixed4 Color : COLOR;
		        float2 TexCoord : TEXCOORD0;
                float2 OverlayTexCoord : TEXCOORD1;
	        };

	        struct VertexOutput
	        {
		        float4 Vertex : SV_POSITION;
		        fixed4 Color : COLOR;
		        float2 TexCoord : TEXCOORD0;
                float2 OverlayTexCoord : TEXCOORD1;
	        };

            inline float3 ApplyHue(float3 aColor, float aHue)
            {
                float angle = radians(aHue);
                float3 k = float3(0.57735, 0.57735, 0.57735);
                float cosAngle = cos(angle);

                return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
            }

            inline float4 ApplyHsbcEffect(float4 color, fixed4 hsbc)
            {
                float hue = 360 * hsbc.r;
                float saturation = hsbc.g + 1;
                float brightness = hsbc.b;
                float contrast = hsbc.a + 1;
                float4 outputColor = color;
                outputColor.rgb = ApplyHue(outputColor.rgb, hue);
                outputColor.rgb = (outputColor.rgb - 0.5) * contrast + 0.5 + brightness;
                outputColor.rgb = lerp(Luminance(outputColor.rgb), outputColor.rgb, saturation);
               
                return outputColor;
            }
			
	        VertexOutput ComputeVertex (VertexInput vertexInput)
	        {
		        VertexOutput vertexOutput;
				
		        vertexOutput.Vertex = ObjectToClipPos(vertexInput.Vertex);
		        vertexOutput.TexCoord = vertexInput.TexCoord;
                vertexOutput.OverlayTexCoord = TRANSFORM_TEX(vertexInput.OverlayTexCoord, _OverlayTex); 
		        vertexOutput.Color = vertexInput.Color;
		        #ifdef PIXELSNAP_ON
		        vertexOutput.Vertex = UnityPixelSnap(vertexOutput.Vertex);
		        #endif
							
		        return vertexOutput;
	        }
			
			fixed4 ComputeFragment (VertexOutput vertexOutput) : SV_Target
			{
				fixed4 texColor = tex2D(_MainTex, vertexOutput.TexCoord) * vertexOutput.Color;
				fixed4 grabColor = tex2D(_OverlayTex, vertexOutput.OverlayTexCoord) * _OverlayTint;
                grabColor.a *= smoothstep(0.0, 1.0, _OpacityRamp / (1.0 - vertexOutput.TexCoord.y));
				
				#include "../BlendOps.cginc"

                texColor *= _MainTint; // Tint main texture after blending to avoid affecting overlay color.
                texColor.rgb = ApplyHsbcEffect(texColor, fixed4(_Hue, _Saturation, _Brightness, _Contrast));

                blendResult = lerp(texColor, blendResult, grabColor.a);

				return blendResult;
			}
			
			ENDCG
		}
	}
	
	Fallback "Sprites/Default"
	CustomEditor "BmMaterialEditor"
}
