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
    }

    private void OnDisable ()
    {
        spriteGlow.enabled = false;
    }
}
