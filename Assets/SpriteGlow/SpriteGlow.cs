using UnityEngine;

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

    [SerializeField, ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
    private Color _glowColor = Color.white;
    [SerializeField, Range(0, 160)]
    private int _outlineWidth = 1;

    [SerializeField, HideInInspector]
    private static Material spriteOutlineMaterial;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialProperties;

    private void OnEnable ()
    {
        if (!spriteOutlineMaterial) spriteOutlineMaterial = new Material(Shader.Find("Sprites/Outline"));
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sharedMaterial = spriteOutlineMaterial;
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
        materialProperties.SetFloat("_IsOutlineEnabled", isOutlineEnabled ? 1f : 0f);
        materialProperties.SetColor("_OutlineColor", GlowColor);
        materialProperties.SetFloat("_OutlineSize", OutlineWidth);
        spriteRenderer.SetPropertyBlock(materialProperties);
    }
}
