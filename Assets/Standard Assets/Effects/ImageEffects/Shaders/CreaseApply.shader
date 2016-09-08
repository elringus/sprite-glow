

Shader "Hidden/CreaseApply" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_HrDepthTex ("Base (RGB)", 2D) = "white" {}
	_LrDepthTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _HrDepthTex;
sampler2D _LrDepthTex;

uniform float4 _MainTex_TexelSize;
half4 _MainTex_ST;
half4 _HrDepthTex_ST;
half4 _LrDepthTex_ST;
uniform float intensity;

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv.xy = v.texcoord.xy;
	return o;
}

half4 frag (v2f i) : SV_Target
{
	float4 hrDepth = tex2D(_HrDepthTex, UnityStereoScreenSpaceUVAdjust(i.uv, _HrDepthTex_ST));
	float4 lrDepth = tex2D(_LrDepthTex, UnityStereoScreenSpaceUVAdjust(i.uv, _LrDepthTex_ST));
	
	hrDepth.a = DecodeFloatRGBA(hrDepth);
	lrDepth.a = DecodeFloatRGBA(lrDepth);
	
	float4 color = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
	
	return color * (1.0-abs(hrDepth.a-lrDepth.a)*intensity);
}

ENDCG


	}
}

Fallback off

}
