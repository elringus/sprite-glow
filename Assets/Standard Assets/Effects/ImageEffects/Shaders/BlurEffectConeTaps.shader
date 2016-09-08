Shader "Hidden/BlurEffectConeTap" {
	Properties { _MainTex ("", any) = "" {} }
	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half2 taps[4] : TEXCOORD1; 
	};
	sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	half4 _MainTex_ST;
	half4 _BlurOffsets;
	v2f vert( appdata_img v ) {
		v2f o; 
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

		o.uv = v.texcoord - _BlurOffsets.xy * _MainTex_TexelSize.xy; // hack, see BlurEffect.cs for the reason for this. let's make a new blur effect soon
#ifdef UNITY_SINGLE_PASS_STEREO
		// we need to keep texel size correct after the uv adjustment.
		o.taps[0] = UnityStereoScreenSpaceUVAdjust(o.uv + _MainTex_TexelSize * _BlurOffsets.xy * (1.0f / _MainTex_ST.xy), _MainTex_ST);
		o.taps[1] = UnityStereoScreenSpaceUVAdjust(o.uv - _MainTex_TexelSize * _BlurOffsets.xy * (1.0f / _MainTex_ST.xy), _MainTex_ST);
		o.taps[2] = UnityStereoScreenSpaceUVAdjust(o.uv + _MainTex_TexelSize * _BlurOffsets.xy * half2(1, -1) * (1.0f / _MainTex_ST.xy), _MainTex_ST);
		o.taps[3] = UnityStereoScreenSpaceUVAdjust(o.uv - _MainTex_TexelSize * _BlurOffsets.xy * half2(1, -1) * (1.0f / _MainTex_ST.xy), _MainTex_ST);
#else
		o.taps[0] = o.uv + _MainTex_TexelSize * _BlurOffsets.xy;
		o.taps[1] = o.uv - _MainTex_TexelSize * _BlurOffsets.xy;
		o.taps[2] = o.uv + _MainTex_TexelSize * _BlurOffsets.xy * half2(1,-1);
		o.taps[3] = o.uv - _MainTex_TexelSize * _BlurOffsets.xy * half2(1,-1);
#endif
		return o;
	}
	half4 frag(v2f i) : SV_Target {
		half4 color = tex2D(_MainTex, i.taps[0]);
		color += tex2D(_MainTex, i.taps[1]);
		color += tex2D(_MainTex, i.taps[2]);
		color += tex2D(_MainTex, i.taps[3]);
		return color * 0.25;
	}
	ENDCG
	SubShader {
		 Pass {
			  ZTest Always Cull Off ZWrite Off

			  CGPROGRAM
			  #pragma vertex vert
			  #pragma fragment frag
			  ENDCG
		  }
	}
	Fallback off
}
