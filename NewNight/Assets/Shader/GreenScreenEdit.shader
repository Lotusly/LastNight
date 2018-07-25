// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Costume/GreenScreenEdit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Cutoff ("Green Cutoff", Range(0,1)) = 0.5 //use this parameter to test transparent
		_BeginDim("Green value to go dim",Range(0,1))=0
		_CutoffLow("Luminance Cutoff Low",Range(0,1)) = 0
		_CutoffHigh("Luminance Cutoff High",Range(0,1))=1
		_GreenHueMin ("Green Hue Min", Range(0, 1)) = 0.234375
		_GreenHueMax ("Green Hue Max", Range(0, 1)) = 0.5078125
		
		//_DistinguishColor("Color to distinguish parts",Color) = (1,1,1,1)
		[Toggle(_ORIGINALCOLOR_ON)] _OriginalColor("Use Original Color",float)=0
		
		_ColorBoundary("Value to distinguish parts",Range(0,1))=0.3
		_UpperMask("Bright Color",Color)=(1,1,1,1)
		[Toggle(_UPPERFIX_ON)] _UpperFix("Fixed color",float)=0
		_LowerMask("Dim Color",Color)=(1,1,1,1)
		[Toggle(_LOWERFIX_ON)] _LowerFix("Fixed color",float)=0
		
	}

	SubShader
	{
		Tags
		{ 
			//"Queue"="ALphaTest" 
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
		
		Pass
		{

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma shader_feature _UPPERFIX_ON
			#pragma shader_feature _LOWERFIX_ON
			#pragma shader_feature _ORIGINALCOLOR_ON
			
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
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
			fixed _Cutoff;
			fixed _BeginDim;
			fixed _ColorBoundary;
			fixed4 _UpperMask;
			fixed4 _LowerMask;
			bool UpperFix;
			bool LowerFix;
			fixed _CutoffLow;
			fixed _CutoffHigh;
			fixed _GreenHueMin, _GreenHueMax;

			// http://chilliant.blogspot.com/2010/11/rgbhsv-in-hlsl.html
			float3 RGBtoHSV(float3 RGB)
			{
				float3 HSV = 0;
				float M = min(RGB.r, min(RGB.g, RGB.b));
				HSV.z = max(RGB.r, max(RGB.g, RGB.b));
				float C = HSV.z - M;
				if (C != 0)
				{
					HSV.y = C / HSV.z;
					float3 D = (((HSV.z - RGB) / 6) + (C / 2)) / C;
					if (RGB.r == HSV.z)
					{
						HSV.x = D.b - D.g;
					}
					else if (RGB.g == HSV.z)
					{
						HSV.x = (1.0/3.0) + D.r - D.b;
					}
					else if (RGB.b == HSV.z)
					{
						HSV.x = (2.0/3.0) + D.g - D.r;
					}
					if ( HSV.x < 0.0 ) { HSV.x += 1.0; }
					if ( HSV.x > 1.0 ) { HSV.x -= 1.0; }
				}
				return HSV;
			}

			float avgColors(fixed4 c, float rp, float bp)
			{
				float avg = (c.r * rp + c.b * bp) / (rp + bp);
				if(c.g > avg)
				{
					return avg;
				}

				return c.g;
			}

			float blueLimit(float4 c)
			{
				if(c.g > c.b)
				{
					return c.b;
				}

				return c.g;
			}

			// https://benmcewan.com/blog/2018/05/20/understanding-despill-algorithms/
			#define CS_BLUE_AVG_MIX 1
			float despillGreen(fixed4 c)
			{
			#ifdef CS_AVG
				return avgColors(c, 1.0f, 1.0f);
			#elif CS_DB_AVG
				return avgColors(c, 1.0f, 2.0f);
			#elif CS_RB_AVG
				return avgColors(c, 2.0f, 1.0f);
			#elif CS_BLUE_LIMIT
				return blueLimit(c);
			#elif CS_RED_LIMIT
				if(c.g > c.r)
				{
					return c.r;
				}
			#elif CS_BLUE_AVG_MIX
				return lerp(blueLimit(c), avgColors(c, 1.0f, 1.0f), 0.5f);
			#endif

				return c.g;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				fixed4 origC = c;

				// compute hsv for green screen check
				fixed3 hsv = RGBtoHSV(c.rgb);
				if(
					hsv.x >= _GreenHueMin && 
					hsv.x <= _GreenHueMax && 
					hsv.y >= 0.4f && 
					hsv.z >= 0.3f)
				{
					discard;
				}

				if(
					hsv.x >= _GreenHueMin &&
					hsv.x <= _GreenHueMax &&
					hsv.y >= 0.15f &&
					hsv.z >= 0.15f)
				{
					if((c.r * c.b) != 0 && (c.g * c.g) / (c.r * c.b) >= 1.5f)
					{
						c.r *= 1.4f;
						c.b *= 1.4f;
					}
					else
					{
						c.r *= 1.2f;
						c.b *= 1.2f;
					}
				}

				// perform despill
				c.g = despillGreen(c);

				float luminance=0.2125*origC.r + 0.7154*origC.g + 0.0721*origC.b;
				if(
					origC.g - origC.r > _Cutoff && 
					origC.g - origC.b > _Cutoff || 
					luminance < _CutoffLow || 
					luminance > _CutoffHigh){
				   discard;
				}
				else if(c.g-c.r>_BeginDim && c.g-c.b>_BeginDim){
				    c.rgba *=(max(_Cutoff-c.g+c.r,_Cutoff-c.g+c.b)/(_Cutoff-_BeginDim));
				    //c.g*=c.a;
				}
				#if _ORIGINALCOLOR_ON
				    return c;
				#endif
				fixed tmp = (c.r+c.b+c.g)/3;
				if(tmp<_ColorBoundary) {
				    #if _LOWERFIX_ON
				        //c.r=0.8;c.b=0.1;c.g=0.1;c.a=0.85;
				        c.rgba=_LowerMask.rgba;
				    #else
				        c.r=min(1,tmp*_LowerMask.r*2);
				        c.g=min(1,tmp*_LowerMask.g*2);
				        c.b=min(1,tmp*_LowerMask.b*2);
				        c.a=_LowerMask.a;
				    #endif
				    
				}
				else{
				    #if _UPPERFIX_ON
				        c.rgba=_UpperMask.rgba;
				    #else
				        c.r=min(1,tmp*_UpperMask.r*2);
				        c.g=min(1,tmp*_UpperMask.g*2);
				        c.b=min(1,tmp*_UpperMask.b*2);
				        c.a=_UpperMask.a;
				    #endif
				    
				}
				c.rgb *= c.a;
				
				return c;
			}
		ENDCG
		}
	}
}