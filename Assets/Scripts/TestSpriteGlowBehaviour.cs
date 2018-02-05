using SpriteGlow;
using System.Collections;
using UnityEngine;

/// <summary>
/// Used to test SpriteGlow functionality at runtime.
/// </summary>
public class TestSpriteGlowBehaviour : MonoBehaviour
{
    private IEnumerator Start ()
    {
        var spriteGlow = gameObject.AddComponent<SpriteGlowEffect>();
        yield return new WaitForSeconds(.5f);
        spriteGlow.enabled = false;
        yield return new WaitForSeconds(.5f);
        spriteGlow.enabled = true;
        yield return new WaitForSeconds(.5f);
        spriteGlow.OutlineWidth = 5;
        yield return new WaitForSeconds(.5f);
        spriteGlow.DrawOutside = true;
        yield return new WaitForSeconds(.5f);
        spriteGlow.DrawOutside = false;
        yield return new WaitForSeconds(.5f);
        spriteGlow.GlowColor = Color.red;
        yield return new WaitForSeconds(.5f);
        spriteGlow.GlowBrightness = 1.5f;
        yield return new WaitForSeconds(.5f);
        Destroy(spriteGlow);
    }
}
