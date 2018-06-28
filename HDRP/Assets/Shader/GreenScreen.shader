// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Costume/GreenScreen"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Cutoff ("Green Cutoff", Range(0,1)) = 0.5 //use this parameter to test transparent
		_BeginDim("Green value to go dim",Range(0,1))=0.5
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
			#pragma multi_compile _ PIXELSNAP_ON
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
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

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
			//fixed _UpperFix;
			//fixed _LowerFix;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				if(c.g-c.r>_Cutoff && c.g-c.b>_Cutoff){
				   discard;
				}
				if(c.g-c.r>_BeginDim && c.g-c.b>_BeginDim){
				    c.a = c.a*(max(c.g-c.r-_BeginDim,c.g-c.b-_BeginDim)/(_Cutoff-_BeginDim));
				    //print(c.a);
				    //c.g=c.g*(max(c.g-c.r-_BeginDim,c.g-c.b-_BeginDim)/(_Cutoff-_BeginDim));
				    //c.g=0.5;
				    //c.g=0;
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
				//if(c.r>c.b) c.b=0;
				///if(c.b>c.r) c.r=0;
				c.rgb *= c.a;
				
				return c;
			}
		ENDCG
		}
	}
}