using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Noise/Noise And Grain (Filmic)")]
    public class NoiseAndGrain : PostEffectsBase
    {

        public float intensityMultiplier = 0.25f;

        public float generalIntensity = 0.5f;
        public float blackIntensity = 1.0f;
        public float whiteIntensity = 1.0f;
        public float midGrey = 0.2f;

        public bool dx11Grain = false;
        public float softness = 0.0f;
        public bool monochrome = false;

        public Vector3 intensities = new Vector3(1.0f, 1.0f, 1.0f);
        public Vector3 tiling = new Vector3(64.0f, 64.0f, 64.0f);
        public float monochromeTiling = 64.0f;

        public FilterMode filterMode = FilterMode.Bilinear;

        public Texture2D noiseTexture;

        public Shader noiseShader;
        private Material noiseMaterial = null;

        public Shader dx11NoiseShader;
        private Material dx11NoiseMaterial = null;

        private static float TILE_AMOUNT = 64.0f;

        private Mesh mesh;

        private void Awake()
        {
            mesh = new Mesh();
        }

        public override bool CheckResources()
        {
            CheckSupport(false);

            noiseMaterial = CheckShaderAndCreateMaterial(noiseShader, noiseMaterial);

            if (dx11Grain && supportDX11)
            {
#if UNITY_EDITOR
				dx11NoiseShader = Shader.Find("Hidden/NoiseAndGrainDX11");
#endif
                dx11NoiseMaterial = CheckShaderAndCreateMaterial(dx11NoiseShader, dx11NoiseMaterial);
            }

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false || (null == noiseTexture))
            {
                Graphics.Blit(source, destination);
                if (null == noiseTexture)
                {
                    Debug.LogWarning("Noise & Grain effect failing as noise texture is not assigned. please assign.", transform);
                }
                return;
            }

            softness = Mathf.Clamp(softness, 0.0f, 0.99f);

            if (dx11Grain && supportDX11)
            {
                // We have a fancy, procedural noise pattern in this version, so no texture needed

                dx11NoiseMaterial.SetFloat("_DX11NoiseTime", Time.frameCount);
                dx11NoiseMaterial.SetTexture("_NoiseTex", noiseTexture);
                dx11NoiseMaterial.SetVector("_NoisePerChannel", monochrome ? Vector3.one : intensities);
                dx11NoiseMaterial.SetVector("_MidGrey", new Vector3(midGrey, 1.0f / (1.0f - midGrey), -1.0f / midGrey));
                dx11NoiseMaterial.SetVector("_NoiseAmount", new Vector3(generalIntensity, blackIntensity, whiteIntensity) * intensityMultiplier);

                if (softness > Mathf.Epsilon)
                {
                    RenderTexture rt = RenderTexture.GetTemporary((int)(source.width * (1.0f - softness)), (int)(source.height * (1.0f - softness)));
                    DrawNoiseQuadGrid(source, rt, dx11NoiseMaterial, noiseTexture, mesh, monochrome ? 3 : 2);
                    dx11NoiseMaterial.SetTexture("_NoiseTex", rt);
                    Graphics.Blit(source, destination, dx11NoiseMaterial, 4);
                    RenderTexture.ReleaseTemporary(rt);
                }
                else
                    DrawNoiseQuadGrid(source, destination, dx11NoiseMaterial, noiseTexture, mesh, (monochrome ? 1 : 0));
            }
            else
            {
                // normal noise (DX9 style)

                if (noiseTexture)
                {
                    noiseTexture.wrapMode = TextureWrapMode.Repeat;
                    noiseTexture.filterMode = filterMode;
                }

                noiseMaterial.SetTexture("_NoiseTex", noiseTexture);
                noiseMaterial.SetVector("_NoisePerChannel", monochrome ? Vector3.one : intensities);
                noiseMaterial.SetVector("_NoiseTilingPerChannel", monochrome ? Vector3.one * monochromeTiling : tiling);
                noiseMaterial.SetVector("_MidGrey", new Vector3(midGrey, 1.0f / (1.0f - midGrey), -1.0f / midGrey));
                noiseMaterial.SetVector("_NoiseAmount", new Vector3(generalIntensity, blackIntensity, whiteIntensity) * intensityMultiplier);

                if (softness > Mathf.Epsilon)
                {
                    RenderTexture rt2 = RenderTexture.GetTemporary((int)(source.width * (1.0f - softness)), (int)(source.height * (1.0f - softness)));
                    DrawNoiseQuadGrid(source, rt2, noiseMaterial, noiseTexture, mesh, 2);
                    noiseMaterial.SetTexture("_NoiseTex", rt2);
                    Graphics.Blit(source, destination, noiseMaterial, 1);
                    RenderTexture.ReleaseTemporary(rt2);
                }
                else
                    DrawNoiseQuadGrid(source, destination, noiseMaterial, noiseTexture, mesh, 0);
            }
        }

        static void DrawNoiseQuadGrid(RenderTexture source, RenderTexture dest, Material fxMaterial, Texture2D noise, Mesh mesh, int passNr)
        {
            RenderTexture.active = dest;

            fxMaterial.SetTexture("_MainTex", source);

            GL.PushMatrix();
            GL.LoadOrtho();

            fxMaterial.SetPass(passNr);

            BuildMesh(mesh, source, noise);

            // Reset camera to origin
            Transform cam = Camera.main.transform;
            Vector3 camPos = cam.position;
            Quaternion camRot = cam.rotation;
            cam.position = Vector3.zero;
            cam.rotation = Quaternion.identity;

            // Render mesh
            Graphics.DrawMeshNow(mesh, Matrix4x4.identity);

            // Restore camera
            cam.position = camPos;
            cam.rotation = camRot;

            GL.PopMatrix();
        }

        static void BuildMesh(Mesh mesh, RenderTexture source, Texture2D noise)
        {
            float noiseSize = (noise.width * 1.0f);
            float subDs = (1.0f * source.width) / TILE_AMOUNT;

            float aspectCorrection = (1.0f * source.width) / (1.0f * source.height);
            float stepSizeX = 1.0f / subDs;
            float stepSizeY = stepSizeX * aspectCorrection;

            int meshWidth = (int)Mathf.Ceil(subDs);
            int meshHeight = (int)Mathf.Ceil(1.0f / stepSizeY);

            // only rebuild the vertex info if the screen size has changed
            if (mesh.vertices.Length != meshWidth * meshHeight * 4)
            {
                Vector3[] vertices = new Vector3[meshWidth * meshHeight * 4];
                Vector2[] uv2s = new Vector2[meshWidth * meshHeight * 4];
                int[] triangles = new int[meshWidth * meshHeight * 6];

                int vertexIndex = 0;
                int triangleIndex = 0;
                for (float x1 = 0.0f; x1 < 1.0f; x1 += stepSizeX)
                {
                    for (float y1 = 0.0f; y1 < 1.0f; y1 += stepSizeY)
                    {
                        vertices[vertexIndex] = new Vector3(x1, y1, 0.1f);
                        vertices[vertexIndex + 1] = new Vector3(x1 + stepSizeX, y1, 0.1f);
                        vertices[vertexIndex + 2] = new Vector3(x1 + stepSizeX, y1 + stepSizeY, 0.1f);
                        vertices[vertexIndex + 3] = new Vector3(x1, y1 + stepSizeY, 0.1f);

                        uv2s[vertexIndex] = new Vector2(0.0f, 0.0f);
                        uv2s[vertexIndex + 1] = new Vector2(1.0f, 0.0f);
                        uv2s[vertexIndex + 2] = new Vector2(1.0f, 1.0f);
                        uv2s[vertexIndex + 3] = new Vector2(0.0f, 1.0f);

                        triangles[triangleIndex] = vertexIndex;
                        triangles[triangleIndex + 1] = vertexIndex + 1;
                        triangles[triangleIndex + 2] = vertexIndex + 2;
                        triangles[triangleIndex + 3] = vertexIndex + 0;
                        triangles[triangleIndex + 4] = vertexIndex + 2;
                        triangles[triangleIndex + 5] = vertexIndex + 3;

                        vertexIndex += 4;
                        triangleIndex += 6;
                    }
                }

                mesh.vertices = vertices;
                mesh.uv2 = uv2s;
                mesh.triangles = triangles;
            }

            // rebuild the UV stream that changes over time
            BuildMeshUV0(mesh, meshWidth, meshHeight, noiseSize, noise.width);
        }

        static void BuildMeshUV0(Mesh mesh, int width, int height, float noiseSize, int noiseWidth)
        {
            float texTile = noiseSize / (noiseWidth * 1.0f);
            float texTileMod = 1.0f / noiseSize;

            Vector2[] uvs = new Vector2[width * height * 4];

            int uvIndex = 0;
            for (int i = 0; i < width * height; i++)
            {
                float tcXStart = Random.Range(0.0f, noiseSize);
                float tcYStart = Random.Range(0.0f, noiseSize);

                tcXStart = Mathf.Floor(tcXStart) * texTileMod;
                tcYStart = Mathf.Floor(tcYStart) * texTileMod;

                uvs[uvIndex] = new Vector2(tcXStart, tcYStart);
                uvs[uvIndex + 1] = new Vector2(tcXStart + texTile * texTileMod, tcYStart);
                uvs[uvIndex + 2] = new Vector2(tcXStart + texTile * texTileMod, tcYStart + texTile * texTileMod);
                uvs[uvIndex + 3] = new Vector2(tcXStart, tcYStart + texTile * texTileMod);

                uvIndex += 4;
            }

            mesh.uv = uvs;
        }
    }
}
