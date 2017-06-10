using UnityEngine;

/// <summary>
/// Used to break batching when testing performance.
/// </summary>
[RequireComponent(typeof(SpriteGlow))]
public class GlowColorRandomizer : MonoBehaviour
{
    private SpriteGlow spriteRenderer;

    private void Awake ()
    {
        spriteRenderer = GetComponent<SpriteGlow>();
    }

    private void Start ()
    {
        spriteRenderer.GlowColor = Random.ColorHSV();
    }
}
