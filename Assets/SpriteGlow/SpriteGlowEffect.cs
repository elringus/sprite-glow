using UnityEngine;

namespace SpriteGlow
{
    /// <summary>
    /// Adds an HDR outline over the <see cref="SpriteRenderer"/>'s sprite borders.
    /// Can be used in conjuction with bloom post-processing to create a glow effect.
    /// </summary>
    [AddComponentMenu("Effects/Sprite Glow")]
    [RequireComponent(typeof(SpriteRenderer)), DisallowMultipleComponent, ExecuteInEditMode]
    public class SpriteGlowEffect : MonoBehaviour
    {
        public SpriteRenderer Renderer { get; private set; }
        public Color GlowColor
        {
            get { return _glowColor; }
            set
            {
                if (_glowColor != value)
                {
                    _glowColor = value;
                    SetMaterialProperties();
                }
            }
        }
        public float GlowBrightness
        {
            get { return _glowBrightness; }
            set
            {
                if (_glowBrightness != value)
                {
                    _glowBrightness = value;
                    SetMaterialProperties();
                }
            }
        }
        public int OutlineWidth
        {
            get { return _outlineWidth; }
            set
            {
                if (_outlineWidth != value)
                {
                    _outlineWidth = value;
                    SetMaterialProperties();
                }
            }
        }
        public float AlphaThreshold
        {
            get { return _alphaThreshold; }
            set
            {
                if (_alphaThreshold != value)
                {
                    _alphaThreshold = value;
                    SetMaterialProperties();
                }
            }
        }
        public bool DrawOutside
        {
            get { return _drawOutside; }
            set
            {
                if (_drawOutside != value)
                {
                    _drawOutside = value;
                    SetMaterialProperties();
                }
            }
        }
        public bool EnableInstancing
        {
            get { return _enableInstancing; }
            set
            {
                if (_enableInstancing != value)
                {
                    _enableInstancing = value;
                    SetMaterialProperties();
                }
            }
        }

        [Tooltip("Base color of the glow.")]
        [SerializeField] private Color _glowColor = Color.white;
        [Tooltip("The brightness (power) of the glow."), Range(1, 10)]
        [SerializeField] private float _glowBrightness = 2f;
        [Tooltip("Width of the outline, in texels."), Range(0, 10)]
        [SerializeField] private int _outlineWidth = 1;
        [Tooltip("Threshold to determine sprite borders."), Range(0f, 1f)]
        [SerializeField] private float _alphaThreshold = .01f;
        [Tooltip("Whether the outline should only be drawn outside of the sprite borders. Make sure sprite texture has sufficient transparent space for the required outline width.")]
        [SerializeField] private bool _drawOutside = false;
        [Tooltip("Whether to enable GPU instancing.")]
        [SerializeField] private bool _enableInstancing = false;

        private MaterialPropertyBlock materialProperties;
        private int isOutlineEnabledId;
        private int outlineColorId;
        private int outlineSizeId;
        private int alphaThresholdId;

        private void Awake ()
        {
            Renderer = GetComponent<SpriteRenderer>();
            isOutlineEnabledId = Shader.PropertyToID("_IsOutlineEnabled");
            outlineColorId = Shader.PropertyToID("_OutlineColor");
            outlineSizeId = Shader.PropertyToID("_OutlineSize");
            alphaThresholdId = Shader.PropertyToID("_AlphaThreshold");
        }

        private void OnEnable ()
        {
            SetMaterialProperties();
        }

        private void OnDisable ()
        {
            SetMaterialProperties();
        }

        private void OnValidate ()
        {
            // Update material properties when changing serialized fields with editor GUI.
            SetMaterialProperties();
        }

        private void OnDidApplyAnimationProperties ()
        {
            // Update material properties when changing serialized fields with Unity animation.
            SetMaterialProperties();
        }

        private void SetMaterialProperties ()
        {
            if (!Renderer) return;

            Renderer.sharedMaterial = SpriteGlowMaterial.GetSharedFor(this);

            if (materialProperties == null)
                materialProperties = new MaterialPropertyBlock();

            materialProperties.SetFloat(isOutlineEnabledId, isActiveAndEnabled ? 1 : 0);
            materialProperties.SetColor(outlineColorId, GlowColor * GlowBrightness);
            materialProperties.SetFloat(outlineSizeId, OutlineWidth);
            materialProperties.SetFloat(alphaThresholdId, AlphaThreshold);

            Renderer.SetPropertyBlock(materialProperties);
        }
    }
}
