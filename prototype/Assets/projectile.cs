using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float knockbackForce = 5f;
    public LayerMask enemyLayers;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            // Apply damage
            EnemyStats stats = other.GetComponent<EnemyStats>();
            if (stats != null)
                stats.TakeDamage(damage);

            // Apply knockback
            KnockbackHandler knockback = other.GetComponent<KnockbackHandler>();
            if (knockback != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                knockback.ReceiveHit(dir, knockbackForce);
            }

            Destroy(gameObject); // Destroy projectile on hit
        }
        else if (!other.isTrigger) // Hit wall/obstacle
        {
            Destroy(gameObject);
        }
    }
}
