// FakeRadialBlur shader for Unity (v1.0)
// based on this shader: http://unitycoder.com/blog/2012/10/02/fake-godrays-shader/
// which is converted/based on this webgl shader: http://demo.bkcore.com/threejs/webgl_tron_godrays.html
 
Shader "Costume/FakeRadialBlur"
{
Properties
{
tDiffuse ("Base (RGB)", 2D) = "white" {}
fX ("fX", Float) = 0.5
fY ("fY", Float) = 0.5
fExposure ("fExposure", Float) = 0.2
fDecay ("fDecay", Float) = 0.9
fDensity ("fDensity", Float) = 0.0
fWeight ("fWeight", Float) = 0.4
fClamp ("fClamp", Float) = 1.0
_alpha("_alpha",Range(0,1)) = 1.0
//iSamples ("iSamples", Int) = 20
}
SubShader {
Tags { 
"RenderType"="Transparent"
 "RenderType"="Transparent" }
LOD 200
Cull Off
Lighting Off
ZWrite Off
Blend One OneMinusSrcAlpha
 
CGPROGRAM
#pragma target 3.0
#pragma surface surf Lambert alpha
 
sampler2D tDiffuse;
float fX,fY,fExposure,fDecay,fDensity,fWeight,fClamp,iSamples,_alpha;
 
struct Input {
float2 uvtDiffuse;
float4 screenPos;
};
 
void surf (Input IN, inout SurfaceOutput o)
{
int iSamples=100;
float2 vUv = IN.uvtDiffuse;
//vUv *= float2(1,1); // repeat?
float2 deltaTextCoord = float2(vUv - float2(fX,fY));
deltaTextCoord *= 1.0 /  float(iSamples) * fDensity;
float2 coord = vUv;
float illuminationDecay = 1.0;
float4 FragColor;
for(int i=0; i < iSamples ; i++)
{
coord -= deltaTextCoord;
float4 texel = tex2D(tDiffuse, coord);
texel *= illuminationDecay * fWeight;
FragColor += texel;
illuminationDecay *= fDecay;
}
FragColor *= fExposure;
FragColor = clamp(FragColor, 0.0, fClamp);
float4 c = FragColor;

o.Alpha = _alpha*c.a;
o.Albedo = c.rgb;
 
}
ENDCG
}
FallBack "Diffuse"
}