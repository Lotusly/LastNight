// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "UI/FG Video"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_OverlayTex ("Overlay Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		// I: This shifts the source video texture up/down/left/right (should be float2?):
		_SourceOffset ("Source Texture Offset", Vector) = (1.0, 1.0, 0.0, 0.0)

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
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
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"




			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;


	//
	// RGB/HSL/etc. functions, from Asset Store Blend shader.
	// I: Added 2.6.18
	//
	fixed3 RgbToHsl(fixed3 rgb)
	{
	    fixed maxC = max(rgb.r, max(rgb.g, rgb.b));
	    fixed minC = min(rgb.r, min(rgb.g, rgb.b));

	    fixed3 hsl;

	    hsl = (maxC + minC) / 2.0;

	    if (maxC == minC) hsl.x = hsl.y = .0;
	    else
	    {
	        fixed d = maxC - minC;
	        hsl.y = (hsl.z > .5) ? d / (2.0 - maxC - minC) : d / (maxC + minC);

	        if (rgb.r > rgb.g && rgb.r > rgb.b) 
	        	hsl.x = (rgb.g - rgb.b) / d + (rgb.g < rgb.b ? 6.0 : .0);
	        else if (rgb.g > rgb.b) 
	        	hsl.x = (rgb.b - rgb.r) / d + 2.0;
	        else 
	        	hsl.x = (rgb.r - rgb.g) / d + 4.0;

	        hsl.x /= 6.0f;
	    }

	    return hsl;
	}

	fixed HueToRgb(fixed3 pqt)
	{
	    if (pqt.z < .0) pqt.z += 1.0;
	    if (pqt.z > 1.0) pqt.z -= 1.0;
	    if (pqt.z < 1.0 / 6.0) return pqt.x + (pqt.y - pqt.x) * 6.0 * pqt.z;
	    if (pqt.z < 1.0 / 2.0) return pqt.y;
	    if (pqt.z < 2.0 / 3.0) return pqt.x + (pqt.y - pqt.x) * (2.0 / 3.0 - pqt.z) * 6.0;

	    return pqt.x;
	}

	fixed3 HslToRgb (fixed3 hsl)
	{ 
		fixed3 rgb;
		fixed3 pqt;

	    if (hsl.y == 0) rgb = hsl.z; 
	    else
	    {
	        pqt.y = hsl.z < .5 ? hsl.z * (1.0 + hsl.y) : hsl.z + hsl.y - hsl.z * hsl.y;
	        pqt.x = 2.0 * hsl.z - pqt.y;
	        rgb.r = HueToRgb(fixed3(pqt.x, pqt.y, hsl.x + 1.0 / 3.0));
	        rgb.g = HueToRgb(fixed3(pqt.x, pqt.y, hsl.x));
	        rgb.b = HueToRgb(fixed3(pqt.x, pqt.y, hsl.x - 1.0 / 3.0));
	    }

	    return rgb;
	}




			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				// OUT.color = IN.color * _Color;
				OUT.color = IN.color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _OverlayTex;
			float4 _SourceOffset;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = tex2D(_MainTex, float2(IN.texcoord.x + _SourceOffset.x, IN.texcoord.y + _SourceOffset.y) ) * IN.color;

				IN.texcoord.y = IN.texcoord.y;

				//
				// Do two things:
				// 1. Photoshop Color filter (alpha doesn't matter here).
				// 2. Tint with straight color (alpha matters here).
				//

				// Alpha of 0 turns this off for now:
				if (_Color.a>0.0f) {
					// Apply Photoshop "Color" blend mode (I: 2.6.18):
					fixed3 aHsl = RgbToHsl(color.rgb);
					fixed3 bHsl = RgbToHsl( half4(_Color.r, _Color.g, _Color.b, 1.0).rgb );
					fixed3 rHsl = fixed3(bHsl.x, bHsl.y, aHsl.z);
					color = fixed4(HslToRgb(rHsl), 1.0);

					// Tint with color (alpha determines how much is tinted):
					color = color*(1.0f-_Color.a)  + float4(_Color.r, _Color.g, _Color.b, 1.0)*_Color.a;
				}

				color.a = 1.0f-tex2D(_OverlayTex, IN.texcoord).r;
				clip (color.a - 0.01);

				return color;
			}
		ENDCG
		}
	}
}






