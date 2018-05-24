
// Blend mode and blur effect combined for UI objects.

Shader "BlendModes/Extra/UIBlured"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _BlurRadius("Blur Radius", Range(1, 10)) = 7
        _BlurStep("Blur Step", Range(0.1, 10)) = 1
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        GrabPass { }

        Pass
        {
            Name "Default"

            CGPROGRAM

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "../BlendModes.cginc"

            #pragma target 3.5
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            #pragma multi_compile BmDarken BmMultiply BmColorBurn BmLinearBurn BmDarkerColor BmLighten BmScreen BmColorDodge BmLinearDodge BmLighterColor BmOverlay BmSoftLight BmHardLight BmVividLight BmLinearLight BmPinLight BmHardMix BmDifference BmExclusion BmSubtract BmDivide BmHue BmSaturation BmColor BmLuminosity
            #pragma vertex ComputeVertex
            #pragma fragment ComputeFragment

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            fixed4 _Color;
            float _BlurRadius;
            float _BlurStep;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            struct VertexInput
            {
                float4 position : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 grabTexcoord : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput ComputeVertex(VertexInput vertexInput)
            {
                VertexOutput vertexOutput;

                UNITY_SETUP_INSTANCE_ID(vertexInput);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(vertexOutput);
                vertexOutput.worldPosition = vertexInput.position;
                vertexOutput.position = ObjectToClipPos(vertexOutput.worldPosition);
                vertexOutput.texcoord = vertexInput.texcoord;
                vertexOutput.color = vertexInput.color * _Color;

                #if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
                #else
                float scale = 1.0;
                #endif
                vertexOutput.grabTexcoord.xy = (float2(vertexOutput.position.x, vertexOutput.position.y * scale) + vertexOutput.position.w) * 0.5;
                vertexOutput.grabTexcoord.zw = vertexOutput.position.zw;

                return vertexOutput;
            }

            fixed4 ComputeFragment (VertexOutput vertexOutput) : SV_Target
            {
                half4 texColor = (tex2D(_MainTex, vertexOutput.texcoord) + _TextureSampleAdd) * vertexOutput.color;
                texColor.a *= UnityGet2DClipping(vertexOutput.worldPosition.xy, _ClipRect);
                #ifdef UNITY_UI_ALPHACLIP
                clip(texColor.a - 0.001);
                #endif

                #define BLUR_GRABTEX(grabX, grabY) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(vertexOutput.grabTexcoord.x + _GrabTexture_TexelSize.x * grabX, vertexOutput.grabTexcoord.y + _GrabTexture_TexelSize.y * grabY, vertexOutput.grabTexcoord.z, vertexOutput.grabTexcoord.w)))
                half4 blurSum = half4(0, 0, 0, 0);
                blurSum += BLUR_GRABTEX(0.0, 0.0);
                int blurMeasurments = 1;
                [unroll(100)]
                for (float range = _BlurStep; range <= _BlurRadius; range += _BlurStep)
                {
                    blurSum += BLUR_GRABTEX(range, range);
                    blurSum += BLUR_GRABTEX(range, -range);
                    blurSum += BLUR_GRABTEX(-range, range);
                    blurSum += BLUR_GRABTEX(-range, -range);

                    float varRange = range + range * frac(sin(blurSum));
                    blurSum += BLUR_GRABTEX(0, varRange);
                    blurSum += BLUR_GRABTEX(0, -varRange);
                    blurSum += BLUR_GRABTEX(varRange, 0);
                    blurSum += BLUR_GRABTEX(-varRange, 0);

                    blurMeasurments += 8;
                }
                fixed4 grabColor = blurSum / blurMeasurments;

                #include "../BlendOps.cginc"

                if (blendResult.a <= 0.1f) return blendResult;
                else
                {
                    fixed alpha = blendResult.a;
                    blendResult.a = 1.f;
                    return lerp(grabColor, blendResult, alpha);
                }
            }

            ENDCG
        }
    }

    Fallback "UI/Default"
    CustomEditor "BmMaterialEditor"
}
