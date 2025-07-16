using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    private KnockbackHandler knock;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Horizontal movement")]
    public float topSpeed = 8f;   
    public float acceleration = 80f;    
    public float deceleration = 60f;   
    public float velPower = 1f;  
    public float frictionAmount = 0.2f;   

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpCutMultiplier = 0.5f;  
    public float ledgeCoyoteTime = 0.1f; // Time window after leaving ledge where you can still jump
    public float ledgeHangThreshold = 0.5f; // How far player can hang off ledge before falling

    private float moveInput;              
    private bool isGrounded;
    private bool isJumping;
    private float lastGroundedTime;        
    private const float groundedTolerance = 0.1f;
    private bool isCrouching;
    private bool isOnLedge;
    private float originalColliderSizeY;
    private float originalColliderOffsetY;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        knock = GetComponent<KnockbackHandler>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        if (boxCollider != null)
        {
            originalColliderSizeY = boxCollider.size.y;
            originalColliderOffsetY = boxCollider.offset.y;
        }
    }

    void Update()
    {
        // Handle movement input
        moveInput = 0;
        if (Input.GetKey(KeyCode.A)) moveInput = -1;
        if (Input.GetKey(KeyCode.D)) moveInput = 1;

        // Handle crouching
        isCrouching = Input.GetKey(KeyCode.S);
        anim.SetBool("isCrouching", isCrouching);

        // Handle ledge hanging when crouching
        if (isCrouching && isOnLedge)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Stop vertical movement
        }

        // Jump handling with coyote time
        if (Input.GetKeyDown(KeyCode.W)) 
        {
            if ((isGrounded || Time.time < lastGroundedTime + ledgeCoyoteTime) && !knock.IsKnockedBack)
            {
                Jump();
            }
        }
        
        if (Input.GetKeyUp(KeyCode.W))
        {
            OnJumpUp();
        }

        // Sprite flipping
        if (moveInput < 0) spriteRenderer.flipX = true;
        if (moveInput > 0) spriteRenderer.flipX = false;

        // Update grounded tolerance timer
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        CheckLedge();
        ApplyHorizontalMovement();
        ApplyFriction();
        anim.SetBool("isRunning", Mathf.Abs(moveInput) > 0.01f && isGrounded);
    }

    void CheckLedge()
    {
        if (boxCollider == null) return;

        // Check if player is near a ledge
        float rayLength = ledgeHangThreshold;
        Vector2 rayOrigin = (Vector2)transform.position + new Vector2(boxCollider.offset.x, boxCollider.offset.y - boxCollider.size.y/2);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, LayerMask.GetMask("Ground", "OneWayPlatform"));

        isOnLedge = !hit && isGrounded;

        // Adjust collider when crouching
        if (isCrouching)
        {
            boxCollider.size = new Vector2(boxCollider.size.x, originalColliderSizeY * 0.5f);
            boxCollider.offset = new Vector2(boxCollider.offset.x, originalColliderOffsetY - originalColliderSizeY * 0.25f);
        }
        else
        {
            boxCollider.size = new Vector2(boxCollider.size.x, originalColliderSizeY);
            boxCollider.offset = new Vector2(boxCollider.offset.x, originalColliderOffsetY);
        }
    }

    void ApplyHorizontalMovement()
    {
        if (knock.IsKnockedBack) return;

        // Prevent movement while crouching on ledge
        if (isCrouching && isOnLedge)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float targetSpeed = moveInput * topSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);
    }

    void ApplyFriction()
    {
        if (isGrounded && Mathf.Abs(moveInput) < 0.01f && Mathf.Abs(rb.linearVelocity.x) > 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.linearVelocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.linearVelocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    void Jump()
    {
        isGrounded = false;
        isJumping = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetTrigger("Jump");
        anim.SetBool("isGrounded", false);
    }

    void OnJumpUp()
    {
        if (rb.linearVelocity.y > 0 && isJumping)
        {
            float cut = rb.linearVelocity.y * (1 - jumpCutMultiplier);
            rb.AddForce(Vector2.down * cut, ForceMode2D.Impulse);
        }
        isJumping = false;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Ground") || c.gameObject.CompareTag("OneWayPlatform"))
        {
            isGrounded = true;
            isJumping = false;
            lastGroundedTime = Time.time;
            anim.SetBool("isGrounded", true);
        }
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Ground") || c.gameObject.CompareTag("OneWayPlatform"))
        {
            isGrounded = true;
            lastGroundedTime = Time.time;
        }
    }

    void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Ground") || c.gameObject.CompareTag("OneWayPlatform"))
        {
            isGrounded = false;
        }
    }

    // Visualize ledge detection in editor
    void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 rayOrigin = (Vector2)transform.position + new Vector2(boxCollider.offset.x, boxCollider.offset.y - boxCollider.size.y/2);
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * ledgeHangThreshold);
        }
    }
}
