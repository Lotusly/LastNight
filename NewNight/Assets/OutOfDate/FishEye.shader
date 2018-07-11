Shader "Costume/FishEye"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale("Scale", Float) = 2
		_Center("Center",Vector)=(0,0,0,0)

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
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _Brightness;
			half _Saturation;
			half _Contrast;
			float _Scale;
			fixed4 _Center;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = fixed4(0,0,0,1);
                float2 scr = (i.uv-float2(0.5,0.5))*2;
                float z=i.vertex.z/i.vertex.w;
                scr = float2(scr.x*(z*_Scale-sqrt(1-scr.x*scr.x))/z/_Scale,scr.y*(z*_Scale-sqrt(1-scr.y*scr.y))/z/_Scale);
                scr = scr/2+float2(0.5,0.5)+_Center.xy;
              
                col = tex2D(_MainTex, scr);
                

				return col;
			}
			ENDCG
		}
	}
	
	Fallback Off
}
