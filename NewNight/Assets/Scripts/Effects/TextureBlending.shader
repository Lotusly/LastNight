Shader "Hidden/Custom/TextureBlending"
{
    HLSLINCLUDE

        #include "PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_TextureToMix,sampler_TextureToMix);
        float _Blend;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float4 leak = _Blend*SAMPLE_TEXTURE2D(_TextureToMix,sampler_TextureToMix,i.texcoord);
            color=float4(1-(1-color.r)*(1-leak.r),1-(1-color.g)*(1-leak.g),1-(1-color.b)*(1-leak.b),1);
            //float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
            //color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}