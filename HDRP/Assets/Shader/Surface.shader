Shader "Custom/Surface" {
	Properties {
		_StencilValue("Stencil",Range(0,255))=0
		_Color("Color",Color)=(1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass{
		    Material{
		        Diffuse [_Color]
		    }
		    Lighting On
		}
	}
	FallBack "Diffuse"
}
