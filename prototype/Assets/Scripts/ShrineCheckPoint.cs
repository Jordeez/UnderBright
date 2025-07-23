using UnityEngine;
using UnityEngine.Rendering.Universal; // Needed for Light2D

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;

    [Header("Visuals")]
    public SpriteRenderer unlitSprite;
    public SpriteRenderer litSprite;
    public Light2D spotlight; // <- Add reference to spotlight

    private void Start()
    {
        SetVisuals(false); // Ensure unlit at start
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;

            // Update checkpoint data
            CheckpointManager.Instance.SetCheckpoint(transform.position);

            // Update visuals and activate spotlight
            CheckpointManager.Instance.SetActiveCheckpoint(this);
        }
    }

    public void SetVisuals(bool isLit)
    {
        if (litSprite != null) litSprite.enabled = isLit;
        if (unlitSprite != null) unlitSprite.enabled = !isLit;
        if (spotlight != null) spotlight.enabled = isLit; // <- Activate spotlight if lit
    }
}
