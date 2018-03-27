using System.Collections.Generic;
using UnityEngine;

namespace SpriteGlow
{
    public class SpriteGlowMaterial : Material
    {
        public Texture SpriteTexture { get { return mainTexture; } }
        public bool DrawOutside { get { return IsKeywordEnabled(OUTSIDE_MATERIAL_KEYWORD); } }
        public bool InstancingEnabled { get { return enableInstancing; } }

        private const string OUTLINE_SHADER_NAME = "Sprites/Outline";
        private const string OUTSIDE_MATERIAL_KEYWORD = "SPRITE_OUTLINE_OUTSIDE";
        private static readonly Shader OUTLINE_SHADER = Shader.Find(OUTLINE_SHADER_NAME);

        private static List<SpriteGlowMaterial> sharedMaterials = new List<SpriteGlowMaterial>();

        public SpriteGlowMaterial (Texture spriteTexture, bool drawOutside = false, bool instancingEnabled = false)
            : base(OUTLINE_SHADER)
        {
            if (!OUTLINE_SHADER) Debug.LogError(string.Format("{0} shader not found. Make sure the shader is included to the build.", OUTLINE_SHADER_NAME));

            mainTexture = spriteTexture;
            if (drawOutside) EnableKeyword(OUTSIDE_MATERIAL_KEYWORD);
            if (instancingEnabled) enableInstancing = true;
        }

        public static Material GetSharedFor (SpriteGlowEffect spriteGlow)
        {
            for (int i = 0; i < sharedMaterials.Count; i++)
            {
                if (sharedMaterials[i].SpriteTexture == spriteGlow.Renderer.sprite.texture &&
                    sharedMaterials[i].DrawOutside == spriteGlow.DrawOutside &&
                    sharedMaterials[i].InstancingEnabled == spriteGlow.EnableInstancing)
                    return sharedMaterials[i];
            }

            var material = new SpriteGlowMaterial(spriteGlow.Renderer.sprite.texture, spriteGlow.DrawOutside, spriteGlow.EnableInstancing);
            material.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
            sharedMaterials.Add(material);

            return material;
        }
    }
}
