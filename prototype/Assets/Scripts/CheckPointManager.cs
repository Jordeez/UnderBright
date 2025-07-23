using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector2 respawnPosition;
    public GameObject player;

    private Checkpoint activeCheckpoint;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetCheckpoint(Vector2 pos)
    {
        respawnPosition = pos;
    }

    public void SetActiveCheckpoint(Checkpoint newCheckpoint)
    {
        // Unlight previous checkpoint
        if (activeCheckpoint != null && activeCheckpoint != newCheckpoint)
            activeCheckpoint.SetVisuals(false);

        // Light up new one
        activeCheckpoint = newCheckpoint;
        activeCheckpoint.SetVisuals(true);
    }

    public void RespawnPlayer()
    {
        player.transform.position = respawnPosition;
        player.GetComponent<PlayerHealth>().RestoreFullHealth();
        EnemyManager.Instance.RespawnAllEnemies();

    }
}
