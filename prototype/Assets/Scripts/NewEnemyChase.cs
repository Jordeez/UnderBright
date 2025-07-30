using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyChaseAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float chaseRange = 5f;
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private Collider2D col;
    private Transform playerTarget;
    private bool isChasing = false;
    private int currentPoint = 0;
    private bool isGrounded;

    public LayerMask groundLayer; // Set in Inspector

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        // Simple ground check using collider
        isGrounded = col.IsTouchingLayers(groundLayer);

        // Player detection
        Collider2D playerCol = Physics2D.OverlapCircle(transform.position, chaseRange, playerLayer);
        isChasing = playerCol != null;
        playerTarget = playerCol != null ? playerCol.transform : null;
    }

    private void FixedUpdate()
    {
        if (!isGrounded) return; // Only move if touching ground

        if (isChasing && playerTarget != null)
            ChasePlayer();
        else
            Patrol();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPoint];
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPoint.position, patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Flip sprite (reversed) and set scale to 0.5
        if (targetPoint.position.x > transform.position.x)
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
        else
            transform.localScale = new Vector3(0.5f, 0.5f, 1);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPoint++;
            if (currentPoint >= patrolPoints.Length)
                currentPoint = 0;
        }
    }

    private void ChasePlayer()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, playerTarget.position, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Flip sprite (reversed) and set scale to 0.5
        if (playerTarget.position.x > transform.position.x)
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
        else
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
