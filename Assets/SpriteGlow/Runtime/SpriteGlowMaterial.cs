using System.Collections.Generic;
using UnityEngine;

namespace SpriteGlow
{
    public class SpriteGlowMaterial : Material
    {
        public Texture SpriteTexture => mainTexture;
        public bool DrawOutside => IsKeywordEnabled(outsideMaterialKeyword);
        public bool InstancingEnabled => enableInstancing;

        private const string outlineShaderName = "Sprites/Outline";
        private const string outsideMaterialKeyword = "SPRITE_OUTLINE_OUTSIDE";

        private static readonly Shader outlineShader = Shader.Find(outlineShaderName);
        private static readonly List<SpriteGlowMaterial> sharedMaterials = new List<SpriteGlowMaterial>();

        public SpriteGlowMaterial (Texture spriteTexture, bool drawOutside = false, bool instancingEnabled = false)
            : base(outlineShader)
        {
            if (!outlineShader) Debug.LogError($"`{outlineShaderName}` shader not found. Make sure the shader is included to the build.");
            mainTexture = spriteTexture;
            if (drawOutside) EnableKeyword(outsideMaterialKeyword);
            if (instancingEnabled) enableInstancing = true;
        }

        public static SpriteGlowMaterial GetSharedFor (SpriteGlowEffect spriteGlow)
        {
            for (int i = 0; i < sharedMaterials.Count; i++)
                if (IsSharedFor(sharedMaterials[i], spriteGlow))
                    return sharedMaterials[i];
            return CreateSharedFor(spriteGlow);
        }

        private static bool IsSharedFor (SpriteGlowMaterial material, SpriteGlowEffect spriteGlow)
        {
            return material.SpriteTexture == spriteGlow.Renderer.sprite.texture &&
                   material.DrawOutside == spriteGlow.DrawOutside &&
                   material.InstancingEnabled == spriteGlow.EnableInstancing;
        }

        private static SpriteGlowMaterial CreateSharedFor (SpriteGlowEffect spriteGlow)
        {
            var material = new SpriteGlowMaterial(spriteGlow.Renderer.sprite.texture, spriteGlow.DrawOutside, spriteGlow.EnableInstancing);
            material.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
            sharedMaterials.Add(material);
            return material;
        }
    }
}
