using SpriteGlow;
using UnityEngine;

[RequireComponent(typeof(SpriteGlowEffect))]
public class GlowColorRandomizer : MonoBehaviour
{
    private SpriteGlowEffect spriteGlow;

    private void Awake ()
    {
        spriteGlow = GetComponent<SpriteGlowEffect>();
    }

    private void OnEnable ()
    {
        spriteGlow.enabled = true;
        spriteGlow.GlowColor = Random.ColorHSV();
        spriteGlow.GlowBrightness = Random.Range(1.5f, 3.0f);
    }

    private void OnDisable ()
    {
        spriteGlow.enabled = false;
    }
}
