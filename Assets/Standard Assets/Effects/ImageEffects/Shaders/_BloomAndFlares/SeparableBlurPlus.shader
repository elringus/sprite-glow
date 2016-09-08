Shader "Hidden/SeparableBlurPlus" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half4 uv01 : TEXCOORD1;
		half4 uv23 : TEXCOORD2;
		half4 uv45 : TEXCOORD3;
		half4 uv67 : TEXCOORD4;
	};
	
	half4 offsets;
	
	sampler2D _MainTex;
	half4     _MainTex_ST;
		
	v2f vert (appdata_img v) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

		o.uv.xy = v.texcoord.xy;

		o.uv01 = v.texcoord.xyxy + offsets.xyxy * half4(1,1, -1,-1);
		o.uv23 = v.texcoord.xyxy + offsets.xyxy * half4(1,1, -1,-1) * 2.0;
		o.uv45 = v.texcoord.xyxy + offsets.xyxy * half4(1,1, -1,-1) * 3.0;
		o.uv67 = v.texcoord.xyxy + offsets.xyxy * half4(1,1, -1,-1) * 4.5;
		o.uv67 = v.texcoord.xyxy + offsets.xyxy * half4(1,1, -1,-1) * 6.5;

		return o;  
	}
		
	half4 frag (v2f i) : SV_Target {
		half4 color = half4 (0,0,0,0);

		color += 0.225 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
		color += 0.150 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv01.xy, _MainTex_ST));
		color += 0.150 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv01.zw, _MainTex_ST));
		color += 0.110 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv23.xy, _MainTex_ST));
		color += 0.110 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv23.zw, _MainTex_ST));
		color += 0.075 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv45.xy, _MainTex_ST));
		color += 0.075 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv45.zw, _MainTex_ST));
		color += 0.0525 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv67.xy, _MainTex_ST));
		color += 0.0525 * tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv67.zw, _MainTex_ST));
		
		return color;
	} 

	ENDCG
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}
	
Fallback off
	
} // shader
