// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "10mm/VoxelWithColor"
{
	Properties
	{
		_Highlight ("Highlight", Range(0, 1)) = 0
		_Emission ("Emission", Range(1, 10)) = 1
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		struct Input
		{
			float4 color;
			float emission;
		};

		// We have to use a custom vertex function to get access to the vertex
		// texcoord data without actually using a texture.
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;

			// Emission is stored in the x value of the first texcoord.
			o.emission = v.texcoord.x;
		}

		UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(fixed, _Highlight)
#define _Highlight_arr Props
		UNITY_INSTANCING_BUFFER_END(Props)

		fixed _Emission;

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 col = IN.color;
			o.Albedo = col.rgb;
			o.Alpha = col.a;

			fixed highlight = UNITY_ACCESS_INSTANCED_PROP(_Highlight_arr, _Highlight);

			// If highlight is <0.01, use the emission value from the vertex data.
			// Otherwise, lerp between the highlight value and the emission value.
			o.Emission = col.rgb * lerp(IN.emission * _Emission, highlight, step(0.01, highlight) * highlight);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
