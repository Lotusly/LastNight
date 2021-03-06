﻿Shader "Costume/LightLeak"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightLeakTex("Light Leak Texture", 2D) = "white" {}
		_LightLeakDegree("Light Leak Degree", Range(0,2)) = 1
		//_Brightness("Brightness",Float) = 1
		//_Saturation("Saturation",Float)=1
		//_Contrast("Contrast",Float)=1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		//LOD 100

		Pass
		{
		
		    ZTest Always 
		    Cull Off 
		    ZWrite Off
		    
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _LightLeakTex;
			float4 _LightLeakTex_ST;
			half _LightLeakDegree;
			//half _Brightness;
			//half _Saturation;
			//half _Contrast;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _LightLeakTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//float4 col = tex2D(_MainTex, i.uv.xy) + tex2D(_LightLeakTex, i.uv.zw)/3;
				float4 col = tex2D(_MainTex,i.uv.xy);
				fixed4 leak = _LightLeakDegree*tex2D(_LightLeakTex,i.uv.zw);
				
				col=float4(1-(1-col.r)*(1-leak.r),1-(1-col.g)*(1-leak.g),1-(1-col.b)*(1-leak.b),1);

				/*float range = max(max(col.r, col.g),col.b);
				if(range>1){
				    col.rgb=col.rgb/range;
				}*/
				//col.a=1;
				/*
				fixed3 finalColor=col.rgb*_Brightness;
				
				fixed luminance = 0.2125*col.r + 0.7154*col.g + 0.0721*col.b;
				fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
				finalColor = lerp(luminanceColor, finalColor, _Saturation);
				
				fixed3 avgColor = fixed3(0.5,0.5,0.5);
				finalColor = lerp(avgColor, finalColor, _Contrast);
				*/
				
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				
				return fixed4(col);
			}
			ENDCG
		}
	}
	
	Fallback Off
}
