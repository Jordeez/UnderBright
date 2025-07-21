using UnityEngine;
using Cinemachine;



public class PlayerMovement : MonoBehaviour
{
    private float originalGravity;


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
    public float jumpForce = 14f;
    public float jumpCutMultiplier = 0.5f;
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;
    public float ledgeCoyoteTime = 0.1f;
    public float ledgeHangThreshold = 0.5f;

    [Header("Dash")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public float dashInputLockDuration = 0.15f;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTimer;
    private Vector2 dashDirection;
    private float dashInputLockTimer;
    private bool isInputLocked = false;


    [Header("Ghost Trail (Sprite)")]
    public GameObject dashGhostPrefab;
    public float ghostSpawnInterval = 0.05f;
    public float ghostLifetime = 0.3f;
    private float lastGhostTime;

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

    [SerializeField] private CinemachineImpulseSource dashImpulseSource;


    public bool IsDashing()
    {
        return isDashing;
    }

    void Awake()
    {
        originalGravity = rb.gravityScale;

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
        if (isDashing || isInputLocked) return;

        moveInput = 0;
        if (Input.GetKey(KeyCode.A)) moveInput = -1;
        if (Input.GetKey(KeyCode.D)) moveInput = 1;

        isCrouching = Input.GetKey(KeyCode.S);
        anim.SetBool("isCrouching", isCrouching);

        if (isCrouching && isOnLedge)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        if (Input.GetKeyDown(KeyCode.W))
        {
            if ((isGrounded || Time.time < lastGroundedTime + ledgeCoyoteTime) && !knock.IsKnockedBack)
                Jump();
        }

        if (Input.GetKeyUp(KeyCode.W))
            OnJumpUp();

        if (Input.GetKeyDown(KeyCode.K))
            TryDash();

        if (moveInput < 0) spriteRenderer.flipX = true;
        if (moveInput > 0) spriteRenderer.flipX = false;

        if (isGrounded) lastGroundedTime = Time.time;
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            // Apply flat dash velocity with reduced vertical strength if upward
            float actualDashForce = (dashDirection.y > 0.1f) ? dashForce * 0.75f : dashForce;
            rb.linearVelocity = dashDirection * actualDashForce;

            dashTimer += Time.fixedDeltaTime;

            if (Time.time >= lastGhostTime + ghostSpawnInterval)
            {
                SpawnDashGhost();
                lastGhostTime = Time.time;
            }

            if (dashTimer >= dashDuration)
            {
                EndDash();
            }
            return;
        }

        if (isInputLocked)
        {
            dashInputLockTimer += Time.fixedDeltaTime;
            if (dashInputLockTimer >= dashInputLockDuration)
                isInputLocked = false;
        }

        CheckLedge();
        ApplyHorizontalMovement();
        ApplyFriction();
        ApplyBetterJumpGravity();
        anim.SetBool("isRunning", Mathf.Abs(moveInput) > 0.01f && isGrounded);
    }

    void ApplyBetterJumpGravity()
    {
        if (rb.linearVelocity.y < -0.1f)
        {
            rb.gravityScale = fallGravityMultiplier;
        }
        else if (rb.linearVelocity.y > 0.1f && !Input.GetKey(KeyCode.W))
        {
            rb.gravityScale = lowJumpGravityMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    void CheckLedge()
    {
        if (boxCollider == null) return;

        float rayLength = ledgeHangThreshold;
        Vector2 rayOrigin = (Vector2)transform.position + new Vector2(boxCollider.offset.x, boxCollider.offset.y - boxCollider.size.y / 2);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, LayerMask.GetMask("Ground", "OneWayPlatform"));

        isOnLedge = !hit && isGrounded;

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

    void TryDash()
    {
        if (!canDash || knock.IsKnockedBack) return;

        dashDirection = new Vector2(moveInput, Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0)).normalized;

        if (dashDirection == Vector2.zero)
            dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // Handle pure upward dash
        bool isPureUpDash = dashDirection == Vector2.up;

        // Clear upward momentum when dashing up to prevent floaty launches
        if (isPureUpDash)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        // Apply dash velocity
        rb.linearVelocity = dashDirection * dashForce;

        // Screen shake
        if (dashImpulseSource != null)
            dashImpulseSource.GenerateImpulse();

        isDashing = true;
        dashTimer = 0f;
        canDash = false;
        rb.gravityScale = 0f;
        isInputLocked = true;
        dashInputLockTimer = 0f;
        lastGhostTime = Time.time;

        SpawnDashGhost();
        Invoke(nameof(ResetDash), dashCooldown);
    }


    void EndDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravity; // or 1f, if you don't use custom gravity

        // Optional: clamp leftover upward velocity if you still feel it's floaty
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, 2f));
    }




    void ResetDash()
    {
        if (isGrounded) canDash = true;
    }

    void SpawnDashGhost()
    {
        if (dashGhostPrefab == null || spriteRenderer == null) return;

        GameObject ghost = Instantiate(dashGhostPrefab, transform.position, Quaternion.identity);
        SpriteRenderer ghostRenderer = ghost.GetComponent<SpriteRenderer>();

        if (ghostRenderer != null)
        {
            ghostRenderer.sprite = spriteRenderer.sprite;
            ghostRenderer.flipX = spriteRenderer.flipX;
            ghostRenderer.color = new Color(1f, 1f, 1f, 0.6f);
        }

        Destroy(ghost, ghostLifetime);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Ground") || c.gameObject.CompareTag("OneWayPlatform"))
        {
            isGrounded = true;
            isJumping = false;
            lastGroundedTime = Time.time;
            anim.SetBool("isGrounded", true);
            canDash = true;
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

    void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 rayOrigin = (Vector2)transform.position + new Vector2(boxCollider.offset.x, boxCollider.offset.y - boxCollider.size.y / 2);
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * ledgeHangThreshold);
        }
    }
}
