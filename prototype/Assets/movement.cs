using UnityEngine;

public class movement : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    private bool isGrounded;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // Combo system variables
    private bool isAttacking = false;

    // Attack input buffer variables
    private float inputBufferTimer = 0f;
    private bool attack1Buffered = false;
    private bool attack2Buffered = false;
    public float inputBufferTime = 0.25f; // Buffer time for inputs (time between attacks)

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Handle input buffer timer
        if (inputBufferTimer > 0f)
        {
            inputBufferTimer -= Time.deltaTime;
        }

        HandleMovement();
        HandleAttackCombo();
    }

    void HandleMovement()
    {
        Vector2 velocity = myRigidbody.linearVelocity;
        anim.SetBool("isRunning", Mathf.Abs(velocity.x) > 0.1f);

        if (Input.GetKey(KeyCode.A))
        {
            velocity.x = -moveSpeed;
            FlipSprite(true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            velocity.x = moveSpeed;
            FlipSprite(false);
        }
        else
        {
            velocity.x = 0f;
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
            anim.SetTrigger("Jump");
            anim.SetBool("isGrounded", false);
        }

        myRigidbody.linearVelocity = velocity;
    }

    void HandleAttackCombo()
    {
        if (inputBufferTimer <= 0f)
        {
            // If no input in buffer, we clear the buffered flags
            attack1Buffered = false;
            attack2Buffered = false;
        }

        // If player presses attack (J) and we're grounded
        if (Input.GetKeyDown(KeyCode.J) && isGrounded)
        {
            if (!isAttacking)
            {
                // First attack input
                if (!attack1Buffered)
                {
                    attack1Buffered = true;
                    anim.SetTrigger("attack1");
                    inputBufferTimer = inputBufferTime; // Set buffer time for subsequent input
                }
                // If we already buffered attack1, buffer attack2
                else if (attack1Buffered && !attack2Buffered)
                {
                    attack2Buffered = true;
                    anim.SetTrigger("attack2");
                    ResetAttackBuffer();
                }
            }
        }
    }

    void ResetAttackBuffer()
    {
        attack1Buffered = false;
        attack2Buffered = false;
        inputBufferTimer = 0f; // Reset input buffer timer
    }

    void FlipSprite(bool faceLeft)
    {
        Vector3 scale = transform.localScale;
        if ((faceLeft && scale.x > 0) || (!faceLeft && scale.x < 0))
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }
}
