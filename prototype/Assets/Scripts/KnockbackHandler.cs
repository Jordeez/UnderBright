using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackHandler : MonoBehaviour
{
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6f;
    [SerializeField] private float knockbackDuration = 0.15f;

    [Header("Armor")]
    [SerializeField] private int armor = 0;
    [SerializeField] private float hitWindow = 0.6f;

    [Header("Direction")]
    [SerializeField] private bool freezeY = true;

    private Rigidbody2D rb;
    private int bufferedHits;
    private float lastHitTime;
    private Vector2 knockVec;
    private float timer;

    public Vector2 CurrentForce { get { return knockVec; } }
    public bool IsKnockedBack { get { return timer > 0f; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (timer <= 0f) return;

        rb.linearVelocity += knockVec;
        timer -= Time.fixedDeltaTime;

        float t = Mathf.Clamp01(timer / knockbackDuration);
        knockVec = Vector2.Lerp(Vector2.zero, knockVec, t);
    }

    public void ReceiveHit(Vector2 attackerPos, float forceOverride = -1f)
    {
        if (armor > 0)
        {
            if (Time.time - lastHitTime > hitWindow)
            {
                bufferedHits = 0;
            }

            bufferedHits++;
            lastHitTime = Time.time;

            if (bufferedHits < armor)
            {
                return;
            }

            bufferedHits = 0;
        }

        float f = (forceOverride > 0f) ? forceOverride : knockbackForce;
        Vector2 dir = ((Vector2)transform.position - attackerPos).normalized;
        // Knockback direction
        if (dir == Vector2.zero)
        {
            dir = Vector2.right * Mathf.Sign(transform.position.x - attackerPos.x);
        }

        // Slight knock-up
        if (freezeY)
        {
            dir.y = 0.25f; // Add a slight upward force even if Y is frozen
        }


        knockVec = dir * f;
        timer = knockbackDuration;
    }
}
