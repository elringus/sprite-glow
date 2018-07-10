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
        spriteGlow.GlowBrightness = Random.Range(2f, 4f);
    }

    private void OnDisable ()
    {
        spriteGlow.enabled = false;
    }
}
