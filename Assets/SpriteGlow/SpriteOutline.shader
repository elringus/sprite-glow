// Draws an HDR outline over the sprite borders. 
// Based on Sprites/Default shader from Unity 2017.2.

Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        [HideInInspector] _IsOutlineEnabled("Enable Outline", float) = 0
        [HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,1,1)
        [HideInInspector] _OutlineSize("Outline Size", float) = 1
        [HideInInspector] _AlphaThreshold("Alpha Threshold", Float) = 0.01
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
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma multi_compile _ SPRITE_OUTLINE_OUTSIDE

            #ifndef SAMPLE_DEPTH_LIMIT
            #define SAMPLE_DEPTH_LIMIT 10
            #endif

            #ifdef UNITY_INSTANCING_ENABLED
            UNITY_INSTANCING_CBUFFER_START(PerDrawSprite)
            fixed4 unity_SpriteRendererColorArray[UNITY_INSTANCED_ARRAY_SIZE];
            float4 unity_SpriteFlipArray[UNITY_INSTANCED_ARRAY_SIZE];
            UNITY_INSTANCING_CBUFFER_END
            #define _RendererColor unity_SpriteRendererColorArray[unity_InstanceID]
            #define _Flip unity_SpriteFlipArray[unity_InstanceID]
            #endif 

            UNITY_INSTANCING_CBUFFER_START(SpriteOutline)
            UNITY_DEFINE_INSTANCED_PROP(float, _IsOutlineEnabled)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _OutlineColor)
            UNITY_DEFINE_INSTANCED_PROP(float, _OutlineSize)
            UNITY_DEFINE_INSTANCED_PROP(float, _AlphaThreshold)
            UNITY_INSTANCING_CBUFFER_END

            CBUFFER_START(UnityPerDrawSprite)
            #ifndef UNITY_INSTANCING_ENABLED
            fixed4 _RendererColor;
            float4 _Flip;
            #endif
            float _EnableExternalAlpha;
            CBUFFER_END

            sampler2D _MainTex, _AlphaTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;

            struct VertexInput
            {
                float4 Vertex : POSITION;
                float4 Color : COLOR;
                float2 TexCoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 Vertex : SV_POSITION;
                fixed4 Color : COLOR;
                float2 TexCoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput ComputeVertex(VertexInput vertexInput)
            {
                VertexOutput vertexOutput;

                UNITY_SETUP_INSTANCE_ID(vertexInput);
                UNITY_TRANSFER_INSTANCE_ID(vertexInput, vertexOutput);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(vertexOutput);

                #ifdef UNITY_INSTANCING_ENABLED
                vertexInput.Vertex.xy *= _Flip.xy;
                #endif

                vertexOutput.Vertex = UnityObjectToClipPos(vertexInput.Vertex);
                vertexOutput.TexCoord = vertexInput.TexCoord;
                vertexOutput.Color = vertexInput.Color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                vertexOutput.Vertex = UnityPixelSnap(vertexOutput.Vertex);
                #endif

                return vertexOutput;
            }

            // Determines whether _OutlineColor should replace sampledColor at the given texCoord when drawing inside the sprite borders.
            // Will return 1 when the test is positive (should draw outline), 0 otherwise.
            int ShouldDrawOutlineInside (fixed4 sampledColor, float2 texCoord, int isOutlineEnabled, int outlineSize, float alphaThreshold)
            {
                // Won't draw if effect is disabled, outline size is zero or sampled fragment is tranpsarent.
                if (isOutlineEnabled * outlineSize * sampledColor.a == 0) return 0;

                // Looking for a transparent pixel (sprite border from inside) around computed fragment with given depth (_OutlineSize).
                // Also checking if sampled fragment is out of the texture space (UV is out of 0-1 range); considering such fragment as sprite border.
                for (int i = 1; i <= SAMPLE_DEPTH_LIMIT; i++)
                {
                    float2 pixelUpTexCoord = texCoord + float2(0, i * _MainTex_TexelSize.y);
                    fixed pixelUpAlpha = pixelUpTexCoord.y > 1.0 ? 0.0 : tex2D(_MainTex, pixelUpTexCoord).a;
                    if (pixelUpAlpha <= alphaThreshold) return 1;

                    float2 pixelDownTexCoord = texCoord - float2(0, i * _MainTex_TexelSize.y);
                    fixed pixelDownAlpha = pixelDownTexCoord.y < 0.0 ? 0.0 : tex2D(_MainTex, pixelDownTexCoord).a;
                    if (pixelDownAlpha <= alphaThreshold) return 1;

                    float2 pixelRightTexCoord = texCoord + float2(i * _MainTex_TexelSize.x, 0);
                    fixed pixelRightAlpha = pixelRightTexCoord.x > 1.0 ? 0.0 : tex2D(_MainTex, pixelRightTexCoord).a;
                    if (pixelRightAlpha <= alphaThreshold) return 1;

                    float2 pixelLeftTexCoord = texCoord - float2(i * _MainTex_TexelSize.x, 0);
                    fixed pixelLeftAlpha = pixelLeftTexCoord.x < 0.0 ? 0.0 : tex2D(_MainTex, pixelLeftTexCoord).a;
                    if (pixelLeftAlpha <= alphaThreshold) return 1;

                    if (i > outlineSize) break;
                }

                return 0;
            }

            // Determines whether _OutlineColor should replace sampledColor at the given texCoord when drawing outside the sprite borders.
            // Will return 1 when the test is positive (should draw outline), 0 otherwise.
            int ShouldDrawOutlineOutside (fixed4 sampledColor, float2 texCoord, int isOutlineEnabled, int outlineSize, float alphaThreshold)
            {
                // Won't draw if effect is disabled, outline size is zero or sampled fragment is above alpha threshold.
                if (isOutlineEnabled * outlineSize == 0) return 0;
                if (sampledColor.a > alphaThreshold) return 0;

                // Looking for an opaque pixel (sprite border from outise) around computed fragment with given depth (_OutlineSize).
                for (int i = 1; i <= SAMPLE_DEPTH_LIMIT; i++)
                {
                    float2 pixelUpTexCoord = texCoord + float2(0, i * _MainTex_TexelSize.y);
                    fixed pixelUpAlpha = tex2D(_MainTex, pixelUpTexCoord).a;
                    if (pixelUpAlpha > alphaThreshold) return 1;

                    float2 pixelDownTexCoord = texCoord - float2(0, i * _MainTex_TexelSize.y);
                    fixed pixelDownAlpha = tex2D(_MainTex, pixelDownTexCoord).a;
                    if (pixelDownAlpha > alphaThreshold) return 1;

                    float2 pixelRightTexCoord = texCoord + float2(i * _MainTex_TexelSize.x, 0);
                    fixed pixelRightAlpha = tex2D(_MainTex, pixelRightTexCoord).a;
                    if (pixelRightAlpha > alphaThreshold) return 1;

                    float2 pixelLeftTexCoord = texCoord - float2(i * _MainTex_TexelSize.x, 0);
                    fixed pixelLeftAlpha = tex2D(_MainTex, pixelLeftTexCoord).a;
                    if (pixelLeftAlpha > alphaThreshold) return 1;

                    if (i > outlineSize) break;
                }

                return 0;
            }

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D(_AlphaTex, uv);
                color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
                #endif

                return color;
            }

            fixed4 ComputeFragment(VertexOutput vertexOutput) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(vertexOutput);

                fixed4 color = SampleSpriteTexture(vertexOutput.TexCoord) * vertexOutput.Color;
                color.rgb *= color.a;

                int isOutlineEnabled = UNITY_ACCESS_INSTANCED_PROP(_IsOutlineEnabled);
                fixed4 outlineColor = UNITY_ACCESS_INSTANCED_PROP(_OutlineColor);
                int outlineSize = UNITY_ACCESS_INSTANCED_PROP(_OutlineSize);
                float alphaThreshold = UNITY_ACCESS_INSTANCED_PROP(_AlphaThreshold);

                #ifdef SPRITE_OUTLINE_OUTSIDE
                int shouldDrawOutline = ShouldDrawOutlineOutside(color, vertexOutput.TexCoord, isOutlineEnabled, outlineSize, alphaThreshold);
                #else
                int shouldDrawOutline = ShouldDrawOutlineInside(color, vertexOutput.TexCoord, isOutlineEnabled, outlineSize, alphaThreshold);
                #endif

                color = lerp(color, outlineColor * outlineColor.a, shouldDrawOutline);

                return color;
            }

            ENDCG
        }
    }
}
