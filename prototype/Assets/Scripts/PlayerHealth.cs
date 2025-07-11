using UnityEngine;
using UnityEngine.InputSystem;   // PlayerInput
using Cinemachine;              // CinemachineImpulseSource

[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    private Animator anim;
    private CinemachineImpulseSource impulse;   // ← declare the field

    public HealthBar healthBar;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        impulse = GetComponent<CinemachineImpulseSource>(); // same GameObject
        healthBar.SetMaxHealth(stats.playerHealth);
    }

    public void TakeDamage(int amount)
    {
        stats.playerHealth = Mathf.Max(stats.playerHealth - amount, 0);
        Debug.Log($"{gameObject.name} took {amount} dmg — HP: {stats.playerHealth}");

        healthBar.SetHealth(stats.playerHealth);

        // Screen shake
        impulse?.GenerateImpulse();   // pass a float to scale by damage if you like

        if (stats.playerHealth == 0)
            Die();
    }

    private void Die()
    {
        // Disable controls
        if (TryGetComponent<PlayerMovement>(out var move)) move.enabled = false;
        if (TryGetComponent<PlayerInput>(out var input)) input.enabled = false;

        // Nudge collider so corpse lines up with ground
        if (TryGetComponent<Collider2D>(out var col))
        {
            Vector2 off = col.offset;
            off.y = 0.12f;   // flip sign if it's still floating
            col.offset = off;
        }

        // Play death animation
        anim.SetTrigger("death");
        Debug.Log($"{gameObject.name} died");
    }
}
