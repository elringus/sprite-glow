using UnityEngine;

/// <summary>
/// Adds an HDR outline over the sprite borders.
/// Can be used in conjuction with a bloom post-processing to create a glow effect.
/// </summary>
[AddComponentMenu("Effects/Sprite Glow")]
[ExecuteInEditMode, RequireComponent(typeof(SpriteRenderer))]
public class SpriteGlow : MonoBehaviour
{
    public Color GlowColor
    {
        get { return _glowColor; }
        set
        {
            if (_glowColor != value)
            {
                _glowColor = value;
                SetMaterialProperties(enabled);
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
                SetMaterialProperties(enabled);
            }
        }
    }

    [Tooltip("Custom material to draw an outline for the sprite.\nIf not provided, will use a default shared one.")]
    public Material CustomOutlineMaterial;

    [Tooltip("Color of the outline. Make sure to set 'Current Brightness' > 1 to enable HDR.")]
    [SerializeField, ColorUsage(true, true, 1f, 10f, 0.125f, 3f)]
    private Color _glowColor = Color.white * 2f;

    [Tooltip("Width of the outline, in texels.")]
    [SerializeField, Range(0, 10)]
    private int _outlineWidth = 1;

    private static Material sharedOutlineMaterial;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialProperties;

    private void OnEnable ()
    {
        if (!sharedOutlineMaterial) sharedOutlineMaterial = new Material(Shader.Find("Sprites/Outline"));
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (CustomOutlineMaterial) spriteRenderer.material = CustomOutlineMaterial;
        else spriteRenderer.sharedMaterial = sharedOutlineMaterial;
        materialProperties = new MaterialPropertyBlock();
        SetMaterialProperties(true);
    }

    private void OnDisable ()
    {
        SetMaterialProperties(false);
    }

    #if UNITY_EDITOR
    private void Update ()
    {
        // Used to control material properties via editor GUI.
        SetMaterialProperties(true);
    }
    #endif

    private void SetMaterialProperties (bool isOutlineEnabled)
    {
        spriteRenderer.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_IsOutlineEnabled", isOutlineEnabled ? 1 : 0);
        materialProperties.SetColor("_OutlineColor", GlowColor);
        materialProperties.SetFloat("_OutlineSize", OutlineWidth);
        spriteRenderer.SetPropertyBlock(materialProperties);
    }
}
