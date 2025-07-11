using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class FlyingType : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float verticalRange = 1f;
    public float horizontalRange = 1f;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 initialPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        initialPosition = transform.position;
    }

    private void Update()
    {
        Move();
        Animate();
    }

    private void Move()
    {
        float xOffset = Mathf.Sin(Time.time * speed) * horizontalRange;
        float yOffset = Mathf.Cos(Time.time * speed) * verticalRange;

        Vector2 newPosition = initialPosition + new Vector2(xOffset, yOffset);
        rb.MovePosition(newPosition);
    }

    private void Animate()
    {
        // Example animation logic
        anim.SetFloat("Speed", rb.linearVelocity.magnitude);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(initialPosition, horizontalRange);
        Gizmos.DrawWireSphere(initialPosition, verticalRange);
        Gizmos.DrawLine(initialPosition + Vector2.up * verticalRange, initialPosition - Vector2.up * verticalRange);
        Gizmos.DrawLine(initialPosition + Vector2.right * horizontalRange, initialPosition - Vector2.right * horizontalRange);
    }
    private void OnEnable()
    {
        initialPosition = transform.position; // Reset position when enabled
    }
    private void OnDisable()
    {
        rb.linearVelocity = Vector2.zero; // Stop movement when disabled
        anim.SetFloat("Speed", 0f); // Reset animation speed
        rb.angularVelocity = 0f; // Stop any rotation
        rb.rotation = 0f; // Reset rotation
        rb.gravityScale = 0f; // Disable gravity if needed
    }
    private void OnDestroy()
    {
        // Cleanup if necessary
        rb = null;
        anim = null;
    }
    private void Reset()
    {
        // Reset to default values
        speed = 2f;
        verticalRange = 1f;
        horizontalRange = 1f;
    }
    private void Start()
    {
        // Initialize position
        initialPosition = transform.position;
        rb.gravityScale = 0f; // Disable gravity for flying objects
    }
    private void FixedUpdate()
    {
        // Ensure physics updates are handled in FixedUpdate
        Move();
    }
    private void LateUpdate()
    {
        // Additional updates after all physics calculations
        Animate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collisions if necessary
        if (collision.gameObject.CompareTag("Player"))
        {
            // Example: Trigger damage or interaction
            Debug.Log("Collided with Player");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle triggers if necessary
        if (other.CompareTag("Player"))
        {
            // Example: Trigger an event or interaction
            Debug.Log("Triggered by Player");
        }
    }
    private void OnBecameVisible()
    {
        // Called when the object becomes visible to any camera
        Debug.Log("Object became visible");
    }

    private void OnBecameInvisible()
    {
        // Called when the object is no longer visible to any camera
        Debug.Log("Object became invisible");
    }

    private void OnApplicationPause(bool pause)
    {
        // Handle application pause state
        if (pause)
        {
            Debug.Log("Application paused");
        }
        else
        {
            Debug.Log("Application resumed");
        }
    }
}