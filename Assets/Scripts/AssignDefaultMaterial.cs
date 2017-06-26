using UnityEngine;

public class AssignDefaultMaterial : MonoBehaviour
{
    private void OnEnable ()
    {
        GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
    }
}
