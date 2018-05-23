
// Blend shader for sprite objects with Unity masks support.

Shader "BlendModes/Extra/MaskableSprite" 
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
	}

	SubShader
	{
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
		
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

		GrabPass { }

		Pass
		{
			CGPROGRAM
			
			#include "UnityCG.cginc"
			#include "../BlendModes.cginc"

			#pragma target 3.0	
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#pragma multi_compile BmDarken BmMultiply BmColorBurn BmLinearBurn BmDarkerColor BmLighten BmScreen BmColorDodge BmLinearDodge BmLighterColor BmOverlay BmSoftLight BmHardLight BmVividLight BmLinearLight BmPinLight BmHardMix BmDifference BmExclusion BmSubtract BmDivide BmHue BmSaturation BmColor BmLuminosity
			#pragma vertex ComputeVertex
			#pragma fragment ComputeFragment
			
			sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _EnableExternalAlpha;
			sampler2D _GrabTexture;
			fixed4 _Color;
			
			struct VertexInput
			{
				float4 Vertex : POSITION;
				fixed4 Color : COLOR;
				float2 TexCoord : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 Vertex : SV_POSITION;
				fixed4 Color : COLOR;
				float2 TexCoord : TEXCOORD0;
				float4 ScreenPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
			};
			
			VertexOutput ComputeVertex (VertexInput vertexInput)
			{
				VertexOutput vertexOutput;
				
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(vertexOutput);

				vertexOutput.Vertex = ObjectToClipPos(vertexInput.Vertex);
				vertexOutput.ScreenPos = vertexOutput.Vertex;	
				vertexOutput.TexCoord = vertexInput.TexCoord;
				#ifdef UNITY_HALF_TEXEL_OFFSET
				vertexOutput.Vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1.0, 1.0);
				#endif
				vertexOutput.Color = vertexInput.Color * _Color;

                #ifdef PIXELSNAP_ON
                vertexOutput.Vertex = UnityPixelSnap(vertexOutput.Vertex);
                #endif
							
				return vertexOutput;
			}

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D(_AlphaTex, uv);
                color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
                #endif

                return color;
            }

			fixed4 ComputeFragment (VertexOutput vertexOutput) : SV_Target
			{
				fixed4 texColor = SampleSpriteTexture(vertexOutput.TexCoord) * vertexOutput.Color;
				texColor.rgb *= texColor.a;
                float2 grabTexCoord = GetGrabTexCoord(vertexOutput.ScreenPos);
				fixed4 grabColor = tex2D(_GrabTexture, grabTexCoord); 
				
				#include "../BlendOps.cginc"

				return blendResult;
			}
			
			ENDCG
		}
	}

	Fallback "Sprites/Default"
	CustomEditor "BmMaterialEditor"
}
