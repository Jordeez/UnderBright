using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    private Animator anim;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Reduces HP and handles death.
    /// </summary>
    /// <param name="amount">Damage to apply.</param>
    public void TakeDamage(int amount)
    {
        stats.playerHealth = Mathf.Max(stats.playerHealth - amount, 0);
        Debug.Log($"{gameObject.name} took {amount} dmg — HP: {stats.playerHealth}");

        if (stats.playerHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: play animation, disable input, trigger respawn, etc.
        anim.SetTrigger("death");
        Debug.Log($"{gameObject.name} died");
        // Destroy(gameObject);          // if you simply want the player gone
    }
}
