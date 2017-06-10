using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(SpriteRenderer))]
public class SpriteGlow : MonoBehaviour
{
    [ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
    public Color GlowColor = Color.white;

    [Range(0, 160)]
    public int OutlineWidth = 1;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialProperties;

    private void OnEnable ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        materialProperties = new MaterialPropertyBlock();
        UpdateOutline(true);
    }

    private void OnDisable ()
    {
        UpdateOutline(false);
    }

    private void Update ()
    {
        UpdateOutline(true);
    }

    private void UpdateOutline (bool isOutlineEnabled)
    {
        spriteRenderer.GetPropertyBlock(materialProperties);
        materialProperties.SetFloat("_IsOutlineEnabled", isOutlineEnabled ? 1f : 0f);
        materialProperties.SetColor("_OutlineColor", GlowColor);
        materialProperties.SetFloat("_OutlineSize", OutlineWidth);
        spriteRenderer.SetPropertyBlock(materialProperties);
    }
}
