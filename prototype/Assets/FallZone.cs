using UnityEngine;

public class FallDeathZone : MonoBehaviour
{
    public float fallThresholdY = -20f; // Set based on your level design
    public GameObject player;

    private PlayerHealth playerHealth;

    private void Start()
    {
        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (player != null && player.transform.position.y < fallThresholdY)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(playerHealth.GetComponent<PlayerStats>().maxHealth); // Instantly kills
            }
        }
    }
}
