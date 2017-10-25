using UnityEngine;

[RequireComponent(typeof(SpriteGlow))]
public class GlowColorRandomizer : MonoBehaviour
{
    private SpriteGlow spriteGlow;

    private void Awake ()
    {
        spriteGlow = GetComponent<SpriteGlow>();
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
