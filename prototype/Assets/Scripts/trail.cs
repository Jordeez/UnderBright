using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color trailColor = new Color(1f, 1f, 1f, 0.5f); 
    void Start()
    {
        spriteRenderer.color = trailColor;
        Destroy(gameObject, 0.5f); 
    }
}
