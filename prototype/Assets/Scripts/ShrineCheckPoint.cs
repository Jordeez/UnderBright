using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;

    [Header("Visuals")]
    public SpriteRenderer unlitSprite;
    public SpriteRenderer litSprite;

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

            // Update visuals
            CheckpointManager.Instance.SetActiveCheckpoint(this);
        }
    }

    public void SetVisuals(bool isLit)
    {
        if (litSprite != null) litSprite.enabled = isLit;
        if (unlitSprite != null) unlitSprite.enabled = !isLit;
    }
}
