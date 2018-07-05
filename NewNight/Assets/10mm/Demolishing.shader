Shader "10mm/Demolishing"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Height("Height",Float) = 0.0
		_Span("Span",FLoat)=1.0
		_Melt("Melt",Range(0,1))=0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
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
				float4 mpos : TEXCOORD1;
			};
			
			struct p
			{
			    float4 vertex : SV_POSITION;
			};
			
			v2f GeneratePoint(float4 vertex, float2 uv, float4 mpos){
			    v2f result;
			    result.vertex=vertex;
			    result.uv=uv;
			    result.mpos = mpos;
			    return result;
			}

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Height;
			float _Span;
			float _Melt;
			
			
			
			v2f vert (appdata v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.mpos = v.vertex;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			[maxvertexcount(15)]
			void geom(triangle v2f input[3], uint pid : SV_PrimitiveID,
                inout TriangleStream<v2f> outStream){
                float4 center = (input[0].mpos+input[1].mpos+input[2].mpos)/3;
                float h = max(0,_Height-center.y);
                float yError = 0.5*_Height/(1+h/_Span);
                float3 pNew = float3(center.x+h*sin(pid),yError,center.z+h*cos(pid));
                if(h==0){
                    pNew.y = center.y-yError;
                }
                
                float x = pNew.x+(input[0].mpos.x-center.x)/(_Melt*h+1);
                float y=pNew.y+(input[0].mpos.y-center.y)/(_Melt*h+1);
                float z = pNew.z+(input[0].mpos.z-center.z)/(_Melt*h+1);
                outStream.Append(GeneratePoint(UnityObjectToClipPos(float4(x,y,z,input[0].mpos.w)),input[0].uv,input[0].mpos));
                

                x=pNew.x+(input[1].mpos.x-center.x)/(_Melt*h+1);
                y=pNew.y+(input[1].mpos.y-center.y)/(_Melt*h+1);
                z = pNew.z+(input[1].mpos.z-center.z)/(_Melt*h+1);
                outStream.Append(GeneratePoint(UnityObjectToClipPos(float4(x,y,z,input[1].mpos.w)),input[1].uv,input[1].mpos));
                

                x=pNew.x+(input[2].mpos.x-center.x)/(_Melt*h+1);
                y=pNew.y+(input[2].mpos.y-center.y)/(_Melt*h+1);
                z = pNew.z+(input[2].mpos.z-center.z)/(_Melt*h+1);
                outStream.Append(GeneratePoint(UnityObjectToClipPos(float4(x,y,z,input[2].mpos.w)),input[2].uv,input[2].mpos));
                
                //outStream.Append(GeneratePoint(UnityObjectToClipPos(input[0].mpos)));
                //outStream.Append(GeneratePoint(UnityObjectToClipPos(input[1].mpos)));
                //outStream.Append(GeneratePoint(UnityObjectToClipPos(input[2].mpos)));
            
            }
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
