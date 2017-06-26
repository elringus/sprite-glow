using UnityEngine;

/// <summary>
/// Adds an HDR outline over the sprite borders.
/// Can be used in conjuction with a bloom post-processing to create a glow effect.
/// </summary>
[AddComponentMenu("Effects/Sprite Glow")]
[ExecuteInEditMode, RequireComponent(typeof(SpriteRenderer))]
public class SpriteGlow : MonoBehaviour
{
    public const string OUTSIDE_MATERIAL_KEYWORD = "SPRITE_OUTLINE_OUTSIDE";

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

    [Tooltip("Whether the outline should only be drawn outside of the sprite borders.\nMake sure sprite texture has sufficient 'free' space for the required outline width.")]
    [SerializeField]
    private bool _drawOutside = false;

    [Tooltip("Custom material to draw an outline for the sprite.\nIf not provided, will use a default shared one.")]
    public Material CustomOutlineMaterial;

    [Tooltip("Color of the outline. Make sure to set 'Current Brightness' > 1 to enable HDR.")]
    [SerializeField, ColorUsage(true, true, 1f, 10f, 0.125f, 3f)]
    private Color _glowColor = Color.white * 2f;

    [Tooltip("Width of the outline, in texels.")]
    [SerializeField, Range(0, 10)]
    private int _outlineWidth = 1;

    [Tooltip("Threshold to determine sprite borders.")]
    [SerializeField, Range(0f, 1f)]
    private float _alphaThreshold = 0.01f;

    private static Material sharedOutlineMaterial;
    private static Material sharedOutsideOutlineMaterial;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialProperties;

    private void OnEnable ()
    {
        if (!sharedOutlineMaterial) sharedOutlineMaterial = new Material(Shader.Find("Sprites/Outline"));
        if (!sharedOutsideOutlineMaterial)
        {
            sharedOutsideOutlineMaterial = new Material(sharedOutlineMaterial.shader);
            sharedOutsideOutlineMaterial.EnableKeyword(OUTSIDE_MATERIAL_KEYWORD);
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        materialProperties = new MaterialPropertyBlock();
        SetMaterialProperties();
    }

    private void OnDisable ()
    {
        SetMaterialProperties();
    }

    private void OnValidate ()
    {
        // Used to control material properties via editor GUI.
        if (isActiveAndEnabled)
            SetMaterialProperties();
    }

    public void SetMaterialProperties ()
    {
        if (!spriteRenderer || materialProperties == null) return;

        if (CustomOutlineMaterial)
        {
            spriteRenderer.material = CustomOutlineMaterial;
            if (DrawOutside && !CustomOutlineMaterial.IsKeywordEnabled(OUTSIDE_MATERIAL_KEYWORD))
                CustomOutlineMaterial.EnableKeyword(OUTSIDE_MATERIAL_KEYWORD);
            else if (!DrawOutside && CustomOutlineMaterial.IsKeywordEnabled(OUTSIDE_MATERIAL_KEYWORD))
                CustomOutlineMaterial.DisableKeyword(OUTSIDE_MATERIAL_KEYWORD);
        }
        else spriteRenderer.sharedMaterial = DrawOutside ? sharedOutsideOutlineMaterial : sharedOutlineMaterial;

        spriteRenderer.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_IsOutlineEnabled", isActiveAndEnabled ? 1 : 0);
        materialProperties.SetColor("_OutlineColor", GlowColor);
        materialProperties.SetFloat("_OutlineSize", OutlineWidth);
        materialProperties.SetFloat("_AlphaThreshold", AlphaThreshold);
        spriteRenderer.SetPropertyBlock(materialProperties);
    }
}
