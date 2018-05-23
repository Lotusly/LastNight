
Shader "LastNightColorTint" {
Properties
{
_MainTex("Base (RGB)", 2D) = "white" {}
_ColorRGB("_ColorRGB", Color) = (1,1,1,1)

}
SubShader
{
	Pass
	{
	Cull Off ZWrite Off ZTest Always
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma target 3.0
	#include "UnityCG.cginc"


	uniform sampler2D _MainTex;
	uniform float _Distortion;
	uniform float4 _ScreenResolution;
	uniform float4 _ColorRGB;
	uniform sampler2D _CameraDepthTexture;
	uniform float _LightIntensity;
	uniform float2 _MainTex_TexelSize;


	struct appdata_t
	{
	float4 vertex   : POSITION;
	float4 color    : COLOR;
	float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
	float2 texcoord  : TEXCOORD0;
	float4 vertex   : SV_POSITION;
	float4 color : COLOR;
	float4 projPos : TEXCOORD1;
	};

	v2f vert(appdata_t IN)
	{
	v2f OUT;
	OUT.vertex = UnityObjectToClipPos(IN.vertex);
	OUT.texcoord = IN.texcoord;
	OUT.color = IN.color;
	OUT.projPos = ComputeScreenPos(OUT.vertex);

	return OUT;
	}

	float4 efx(float2 uv)
	{
	float4 col = 1-tex2D(_MainTex,uv);
	return col;
	}


	half4 _MainTex_ST;
	float4 frag(v2f i) : COLOR
	{
		// return _ColorRGB;

		float4 col = tex2D(_MainTex, i.texcoord);

		float brightness = col.r * 0.3 + col.g * 0.59 + col.b * 0.11 < 0.5;

		col = col*_ColorRGB;

		//if( col.r * 0.3 + col.g * 0.59 + col.b * 0.11 < 0.5 ) {
		//	col.r = ( 1-(1-2*(_ColorRGB.r-0.5)) * (1-col.r) )*0.5 + col.r*0.5;
		//	col.g = ( 1-(1-2*(_ColorRGB.g-0.5)) * (1-col.g) )*0.5 + col.g*0.5;
		//	col.b = ( 1-(1-2*(_ColorRGB.b-0.5)) * (1-col.b) )*0.5 + col.b*0.5;
		//} else {
		//	col.r = saturate(2*col.r*_ColorRGB.r) + col.r*0.5;
		//	col.g = saturate(2*col.g*_ColorRGB.g) + col.g*0.5;
		//	col.b = saturate(2*col.b*_ColorRGB.b) + col.b*0.5;
		//}

		// (Target > 0.5) * (1 - (1-2*(Target-0.5)) * (1-Blend)) + (Target <= 0.5) * ((2*Target) * Blend)


		return col;

	}



	ENDCG
	}

	}

}




