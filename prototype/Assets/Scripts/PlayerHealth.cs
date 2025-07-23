using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    private Animator anim;
    private CinemachineImpulseSource impulse;

    public HealthBar healthBar;

    private Vector2 originalColliderOffset;
    private Collider2D col;
    private PlayerMovement move;
    private PlayerInput input;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        impulse = GetComponent<CinemachineImpulseSource>();
        col = GetComponent<Collider2D>();
        move = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInput>();

        if (col != null)
            originalColliderOffset = col.offset;

        healthBar.SetMaxHealth(stats.maxHealth);
        stats.playerHealth = stats.maxHealth;
        healthBar.SetHealth(stats.playerHealth);
    }

    public void TakeDamage(int amount)
    {
        stats.playerHealth = Mathf.Max(stats.playerHealth - amount, 0);
        Debug.Log($"{gameObject.name} took {amount} dmg — HP: {stats.playerHealth}");

        healthBar.SetHealth(stats.playerHealth);

        impulse?.GenerateImpulse();

        if (stats.playerHealth == 0)
            Die();
    }

    private void Die()
    {
        if (move != null) move.enabled = false;
        if (input != null) input.enabled = false;

        if (col != null)
        {
            Vector2 off = col.offset;
            off.y = 0.12f;
            col.offset = off;
        }

        anim.SetTrigger("death");
        Debug.Log($"{gameObject.name} died");

        // Delay respawn to allow death animation to play
        Invoke(nameof(Respawn), 1.5f); // or use animation event if preferred
    }

    private void Respawn()
    {
        // Reset position to checkpoint
        CheckpointManager.Instance.RespawnPlayer();

        // Restore control
        if (move != null) move.enabled = true;
        if (input != null) input.enabled = true;

        // Restore original collider offset
        if (col != null)
            col.offset = originalColliderOffset;

        // Reset animation state if needed
        anim.ResetTrigger("death");
        anim.SetBool("isDead", false);
        anim.Play("Idle"); // or your default state
    }

    public void RestoreFullHealth()
    {
        stats.playerHealth = stats.maxHealth;
        healthBar.SetHealth(stats.playerHealth);
    }
}
