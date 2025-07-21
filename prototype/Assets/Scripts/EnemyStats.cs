using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Experience")]
    [SerializeField] private int experienceValue = 5;

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Trigger death animation
        if (anim != null)
            anim.SetTrigger("death");

        // Disable all scripts except this one
        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script != this)
                script.enabled = false;
        }

        // Optionally disable colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // Destroy after animation ends (adjust time to your animation)
        Destroy(gameObject, 1f);
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetExperienceValue()
    {
        return experienceValue;
    }
}
