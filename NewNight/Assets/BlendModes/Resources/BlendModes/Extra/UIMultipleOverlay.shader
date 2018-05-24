
// Allows applying multiple overlay textures.

Shader "BlendModes/Extra/UIMultipleOverlay"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OverlayTex1("Overlay Texture 1", 2D) = "white" {}
        _OverlayTex2("Overlay Texture 2", 2D) = "white" {}
        _OverlayTex3("Overlay Texture 3", 2D) = "white" {}
        _OverlayTex4("Overlay Texture 4", 2D) = "white" {}
        _OverlayTex5("Overlay Texture 5", 2D) = "white" {}
        _OverlayColor1("Overlay Color 1", Color) = (1,1,1,0)
        _OverlayColor2("Overlay Color 2", Color) = (1,1,1,0)
        _OverlayColor3("Overlay Color 3", Color) = (1,1,1,0)
        _OverlayColor4("Overlay Color 4", Color) = (1,1,1,0)
        _OverlayColor5("Overlay Color 5", Color) = (1,1,1,0)
		_Color ("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"PreviewType" = "Plane"		
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

            #include "UnityCG.cginc"	
            #include "../BlendModes.cginc"

            #pragma target 3.0			
			#pragma multi_compile BmDarken BmMultiply BmColorBurn BmLinearBurn BmDarkerColor BmLighten BmScreen BmColorDodge BmLinearDodge BmLighterColor BmOverlay BmSoftLight BmHardLight BmVividLight BmLinearLight BmPinLight BmHardMix BmDifference BmExclusion BmSubtract BmDivide BmHue BmSaturation BmColor BmLuminosity
			#pragma vertex ComputeVertex
			#pragma fragment ComputeFragment

	        sampler2D _MainTex;
            sampler2D _MainTex_ST;
	        fixed4 _Color;
            sampler2D _OverlayTex1, _OverlayTex2, _OverlayTex3, _OverlayTex4, _OverlayTex5;
            fixed4 _OverlayColor1, _OverlayColor2, _OverlayColor3, _OverlayColor4, _OverlayColor5;
			
	        struct VertexInput
	        {
		        float4 Vertex : POSITION;
		        fixed4 Color : COLOR;
		        float2 TexCoord : TEXCOORD;
	        };

	        struct VertexOutput
	        {
		        float4 Vertex : SV_POSITION;
		        fixed4 Color : COLOR;
		        float2 TexCoord : TEXCOORD;
	        };
			
	        VertexOutput ComputeVertex (VertexInput vertexInput)
	        {
		        VertexOutput vertexOutput;
				
		        vertexOutput.Vertex = ObjectToClipPos(vertexInput.Vertex);
		        vertexOutput.TexCoord = vertexInput.TexCoord;
		        #ifdef UNITY_HALF_TEXEL_OFFSET
		        vertexOutput.Vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1.0, 1.0);
		        #endif
		        vertexOutput.Color = vertexInput.Color * _Color;
							
		        return vertexOutput;
	        }

			fixed4 ComputeFragment (VertexOutput vertexOutput) : SV_Target
			{
				fixed4 texColor = tex2D(_MainTex, vertexOutput.TexCoord) * vertexOutput.Color;
				clip(texColor.a - .01);

				fixed4 grabColor = tex2D(_OverlayTex1, vertexOutput.TexCoord) * _OverlayColor1;
                #include "../BlendOps.cginc"
                blendResult = lerp(texColor, blendResult, grabColor.a);
                texColor = blendResult;

                #define BLEND_RESULT_DECLARED

                grabColor = tex2D(_OverlayTex2, vertexOutput.TexCoord) * _OverlayColor2;
                #undef BLEND_OPS
                #include "../BlendOps.cginc"
                blendResult = lerp(texColor, blendResult, grabColor.a);
                texColor = blendResult;

                grabColor = tex2D(_OverlayTex3, vertexOutput.TexCoord) * _OverlayColor3;
                #undef BLEND_OPS
                #include "../BlendOps.cginc"
                blendResult = lerp(texColor, blendResult, grabColor.a);
                texColor = blendResult;

                grabColor = tex2D(_OverlayTex4, vertexOutput.TexCoord) * _OverlayColor4;
                #undef BLEND_OPS
                #include "../BlendOps.cginc"
                blendResult = lerp(texColor, blendResult, grabColor.a);
                texColor = blendResult;

                grabColor = tex2D(_OverlayTex5, vertexOutput.TexCoord) * _OverlayColor5;
                #undef BLEND_OPS
                #include "../BlendOps.cginc"
                blendResult = lerp(texColor, blendResult, grabColor.a);
                texColor = blendResult;

				return texColor;
			}
			
			ENDCG
		}
	}

	Fallback "UI/Default"
	CustomEditor "BmMaterialEditor"
}
