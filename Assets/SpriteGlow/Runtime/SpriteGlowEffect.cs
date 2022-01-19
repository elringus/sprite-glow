using System;
using UnityEngine;

namespace SpriteGlow
{
    /// <summary>
    /// Adds an HDR outline over the <see cref="SpriteRenderer"/>'s sprite borders.
    /// Can be used in conjunction with bloom post-processing to create a glow effect.
    /// </summary>
    [AddComponentMenu("Effects/Sprite Glow")]
    [RequireComponent(typeof(SpriteRenderer)), DisallowMultipleComponent, ExecuteInEditMode]
    public class SpriteGlowEffect : MonoBehaviour
    {
        public SpriteRenderer Renderer { get; private set; }
        public Color GlowColor { get => glowColor; set => SetProperty(ref glowColor, value); }
        public float GlowBrightness { get => glowBrightness; set => SetProperty(ref glowBrightness, value); }
        public int OutlineWidth { get => outlineWidth; set => SetProperty(ref outlineWidth, value); }
        public float AlphaThreshold { get => alphaThreshold; set => SetProperty(ref alphaThreshold, value); }
        public bool DrawOutside { get => drawOutside; set => SetProperty(ref drawOutside, value); }
        public bool EnableInstancing { get => enableInstancing; set => SetProperty(ref enableInstancing, value); }

        [Tooltip("Base color of the glow.")]
        [SerializeField] private Color glowColor = Color.white;
        [Tooltip("The brightness (power) of the glow."), Range(1, 10)]
        [SerializeField] private float glowBrightness = 2f;
        [Tooltip("Width of the outline, in texels."), Range(0, 10)]
        [SerializeField] private int outlineWidth = 1;
        [Tooltip("Threshold to determine sprite borders."), Range(0f, 1f)]
        [SerializeField] private float alphaThreshold = .01f;
        [Tooltip("Whether the outline should only be drawn outside of the sprite borders. Make sure sprite texture has sufficient transparent space for the required outline width.")]
        [SerializeField] private bool drawOutside;
        [Tooltip("Whether to enable GPU instancing.")]
        [SerializeField] private bool enableInstancing;

        private static readonly int isOutlineEnabledId = Shader.PropertyToID("_IsOutlineEnabled");
        private static readonly int outlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int outlineSizeId = Shader.PropertyToID("_OutlineSize");
        private static readonly int alphaThresholdId = Shader.PropertyToID("_AlphaThreshold");

        private MaterialPropertyBlock propertyBlock;

        private void Awake () => Renderer = GetComponent<SpriteRenderer>();
        private void OnEnable () => SetMaterialProperties();
        private void OnDisable () => SetMaterialProperties();
        private void OnValidate () => SetMaterialProperties();
        private void OnDidApplyAnimationProperties () => SetMaterialProperties();

        private void SetProperty<T> (ref T current, T value) where T : IEquatable<T>
        {
            if (current.Equals(value)) return;
            current = value;
            SetMaterialProperties();
        }

        private void SetMaterialProperties ()
        {
            if (!Renderer) return;

            Renderer.sharedMaterial = SpriteGlowMaterial.GetSharedFor(this);

            if (propertyBlock is null)
                propertyBlock = new MaterialPropertyBlock();

            propertyBlock.SetFloat(isOutlineEnabledId, isActiveAndEnabled ? 1 : 0);
            propertyBlock.SetColor(outlineColorId, GlowColor * GlowBrightness);
            propertyBlock.SetFloat(outlineSizeId, OutlineWidth);
            propertyBlock.SetFloat(alphaThresholdId, AlphaThreshold);

            Renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
