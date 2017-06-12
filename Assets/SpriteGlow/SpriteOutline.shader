// Draws an HDR outline over the sprite borders.

Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        [Toggle][PerRendererData] _IsOutlineEnabled("Enable Outline", int) = 0
        [HDR][PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
        [PerRendererData] _OutlineSize("Outline Size", int) = 1
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex ComputeVertex
            #pragma fragment ComputeFragment
            #pragma multi_compile _ PIXELSNAP_ON

            fixed4 _Color;
            fixed4 _OutlineColor;
            int _IsOutlineEnabled;
            int _OutlineSize;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            struct VertexInput
            {
                float4 Vertex : POSITION;
                fixed4 Color : COLOR;
                float2 TexCoord : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 Vertex : SV_POSITION;
                fixed4 Color : COLOR;
                float2 TexCoord : TEXCOORD0;
            };

            VertexOutput ComputeVertex (VertexInput vertexInput)
            {
                VertexOutput vertexOutput;

                vertexOutput.Vertex = UnityObjectToClipPos(vertexInput.Vertex);
                vertexOutput.TexCoord = vertexInput.TexCoord;
                vertexOutput.Color = vertexInput.Color * _Color;
                #ifdef PIXELSNAP_ON
                vertexOutput.Vertex = UnityPixelSnap(vertexOutput.Vertex);
                #endif

                return vertexOutput;
            }

            // Determines whether _OutlineColor should replace sampledColor at the given texCoord.
            // Will return 1.0 when the test is positive (should draw outline), 0.0 otherwise.
            int ShouldDrawOutline (fixed4 sampledColor, float2 texCoord)
            {
                // Won't draw if effect is disabled, outline size is zero or sampled fragment is tranpsarent.
                if (_IsOutlineEnabled * _OutlineSize * sampledColor.a == 0) return 0;

                // Looking for a transparent pixel (sprite border) around computed fragment with given depth (_OutlineSize).
                // Also checking if sampled fragment is out of the texture space (UV is out of 0-1 range); considering such fragment as sprite border.
                [unroll(10)]
                for (int i = 1; i < _OutlineSize + 1; i++)
                {
                    float2 pixelUpTexCoord = texCoord + float2(0, i * _MainTex_TexelSize.y);
                    fixed pixelUpAlpha = pixelUpTexCoord.y > 1.0 ? 0.0 : tex2D(_MainTex, pixelUpTexCoord).a;

                    float2 pixelDownTexCoord = texCoord - float2(0, i * _MainTex_TexelSize.y);
                    fixed pixelDownAlpha = pixelDownTexCoord.y < 0.0 ? 0.0 : tex2D(_MainTex, pixelDownTexCoord).a;

                    float2 pixelRightTexCoord = texCoord + float2(i * _MainTex_TexelSize.x, 0);
                    fixed pixelRightAlpha = pixelRightTexCoord.x > 1.0 ? 0.0 : tex2D(_MainTex, pixelRightTexCoord).a;

                    float2 pixelLeftTexCoord = texCoord - float2(i * _MainTex_TexelSize.x, 0);
                    fixed pixelLeftAlpha = pixelLeftTexCoord.x < 0.0 ? 0.0 : tex2D(_MainTex, pixelLeftTexCoord).a;

                    // Combining tests to minimize conditional branching.
                    fixed mAlpha = pixelUpAlpha * pixelDownAlpha * pixelRightAlpha * pixelLeftAlpha;
                    if (mAlpha == 0.0) return 1;
                }

                return 0;
            }

            fixed4 ComputeFragment (VertexOutput vertexOutput) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, vertexOutput.TexCoord) * vertexOutput.Color;
                color *= color.a;

                int shouldDrawOutline = ShouldDrawOutline(color, vertexOutput.TexCoord);
                color = lerp(color, _OutlineColor * _OutlineColor.a, shouldDrawOutline);

                return color;
            }

            ENDCG
        }
    }
}
